#!/usr/bin/env python3
"""
Eupepsia / Soilless Farm Lab -> CRM staging-data extractor.

Reads the two customer source documents and emits auditable staging JSON used by
the backend seeders (CRM.Data/Seeds/InventoryDataSeeder + OperationalDataSeeder).

Sources (override with env vars SRC_DOCX / SRC_XLSX):
  - CRM Role Allocation Document.docx   -> roles.json, departments.json, staff.json
  - SHIPMENTS RECEIVED CARD (1).xlsx    -> categories.json, locations.json, items.json,
                                           inventory-transactions.json

EVERY value that is NOT present verbatim in the source is flagged with a `created`
or `*_inferred` field so it can be reviewed/replaced by the customer. Nothing is
invented beyond what the rows support; unparseable quantities become 0 + a flag.

Run:  python tools/import/extract.py
"""
import os, re, json, zipfile, io, datetime, collections

HERE = os.path.dirname(os.path.abspath(__file__))
OUT  = os.path.normpath(os.path.join(HERE, "..", "..", "CRM.Data", "Seeds", "Data", "Eupepsia-Seed-Data"))
DOWNLOADS = os.path.join(os.path.expanduser("~"), "Downloads")
SRC_DOCX = os.environ.get("SRC_DOCX", os.path.join(DOWNLOADS, "CRM Role Allocation Document.docx"))
SRC_XLSX = os.environ.get("SRC_XLSX", os.path.join(DOWNLOADS, "SHIPMENTS RECEIVED CARD (1).xlsx"))

os.makedirs(OUT, exist_ok=True)

def unesc(s):
    return (s.replace("&amp;", "&").replace("&lt;", "<").replace("&gt;", ">")
             .replace("&quot;", '"').replace("&#39;", "'"))

def write(name, obj):
    p = os.path.join(OUT, name)
    with open(p, "w", encoding="utf-8") as f:
        json.dump(obj, f, ensure_ascii=False, indent=2)
    print(f"  wrote {name:32s} ({len(obj) if isinstance(obj, list) else 1} records)")

# --------------------------------------------------------------------------- #
# ROLES  (6 documented access tiers; permission sets DERIVED from descriptions)
# --------------------------------------------------------------------------- #
def build_roles():
    # NOTE: permission code lists are CREATED (the source gives prose, not codes).
    roles = [
        {"name": "Super Administrator", "code": "SUPER_ADMIN", "docTier": "SUPER ADMIN",
         "description": "All ADMIN privileges plus onboarding/removing users, configuring system settings, maintenance and updates.",
         "permissions": "ALL", "created": {"permissions": True}},
        {"name": "Administrator", "code": "ADMIN", "docTier": "ADMIN",
         "description": "Full operational visibility and access across all modules.",
         "permissions": "ALL_EXCEPT_ADMIN_MGMT", "created": {"permissions": True}},
        {"name": "Manager", "code": "MANAGER", "docTier": "MANAGER",
         "description": "Department-level oversight; first-level approval/rejection for downliner requests.",
         "permissions": "MANAGER", "created": {"permissions": True}},
        {"name": "Manager (Cross-Dept Access)", "code": "MANAGER_ACCESS", "docTier": "MANAGER (ACCESS)",
         "description": "Same approval authority as Manager, scoped to cross-departmental access.",
         "permissions": "MANAGER", "created": {"permissions": True,
            "note": "cross-department scoping is NOT enforced by the system today (flagged G-R3)"}},
        {"name": "Department User", "code": "DEPT_USER", "docTier": "DEPARTMENT USER",
         "description": "Standard user limited to own department functions; no approval rights.",
         "permissions": "DEPT_USER", "created": {"permissions": True}},
        {"name": "User (Artisan)", "code": "ARTISAN", "docTier": "USER (ARTISAN)",
         "description": "Basic operational access for artisan/field staff.",
         "permissions": "ARTISAN", "created": {"permissions": True}},
    ]
    return roles

# --------------------------------------------------------------------------- #
# DOCX staff table -> staff.json + inferred departments.json
# --------------------------------------------------------------------------- #
ROLE_TIER_TO_CODE = {
    "ADMIN": "ADMIN", "SUPER ADMIN": "SUPER_ADMIN", "MANAGER": "MANAGER",
    "MANAGER(ACCESS)": "MANAGER_ACCESS", "MANAGER (ACCESS)": "MANAGER_ACCESS",
    "DEPARTMENT USER": "DEPT_USER", "USER": "ARTISAN",
}
# Position -> inferred department (CREATED: the doc has no department column)
POSITION_TO_DEPT = [
    (r"co{2}|^manager$|admin officer",            "Administration & Management"),
    (r"i\.?c\.?t|data administrator",             "ICT"),
    (r"^hr$|human resource",                      "Human Resources"),
    (r"bdo",                                       "Business Development"),
    (r"relationship|communication|social mobil|media|^cso$", "Relationship & Communications"),
    (r"agronom",                                   "Agronomy"),
    (r"sales",                                      "Sales"),
    (r"audit|finance",                             "Finance & Audit"),
    (r"quality assurance",                         "Quality Assurance"),
    (r"cr.?che|clinic",                            "Crèche/Clinic"),
    (r"operations manager|logistic|procurement|inventory|facility|processing|pack house|driver|electric|plumb|bazuki",
                                                    "Operations"),
]
def infer_dept(position):
    p = position.lower()
    for pat, dept in POSITION_TO_DEPT:
        if re.search(pat, p):
            return dept, True  # (name, inferred)
    return "Operations", True  # fallback, still inferred

def docx_tables(path):
    with zipfile.ZipFile(path) as z:
        xml = z.read("word/document.xml").decode("utf-8")
    tables = []
    for tbl in re.findall(r"<w:tbl>.*?</w:tbl>", xml, re.S):
        rows = []
        for tr in re.findall(r"<w:tr[ >].*?</w:tr>", tbl, re.S):
            cells = []
            for tc in re.findall(r"<w:tc>.*?</w:tc>", tr, re.S):
                # NB: require <w:t> or "<w:t ..." (space) so it does NOT match <w:tcPr>, <w:tcW>, etc.
                txt = "".join(re.findall(r"<w:t(?:\s[^>]*)?>(.*?)</w:t>", tc, re.S))
                cells.append(unesc(txt).strip())
            rows.append(cells)
        tables.append(rows)
    return tables

def build_staff_and_departments():
    tables = docx_tables(SRC_DOCX)
    staff = []
    # staff tables have header starting with "S/N"
    for tbl in tables:
        if not tbl or not tbl[0]:
            continue
        header = [c.upper() for c in tbl[0]]
        if header and header[0].startswith("S/N") and "POSITION" in " ".join(header):
            has_downliner = "DOWNLINERS" in " ".join(header)
            for row in tbl[1:]:
                if len(row) < 4 or not row[0].strip().isdigit():
                    continue
                sn = int(row[0].strip())
                name = row[1].strip()
                position = row[2].strip()
                tier = re.sub(r"\s+", " ", row[3].strip().upper())
                code = ROLE_TIER_TO_CODE.get(tier) or ROLE_TIER_TO_CODE.get(tier.replace(" ", ""))
                dept, dept_inferred = infer_dept(position)
                rec = {
                    "sn": sn, "name": name, "position": position,
                    "docRoleTier": row[3].strip(), "roleCode": code or "DEPT_USER",
                    "departmentName": dept,
                    "downliners": (row[4].strip() if has_downliner and len(row) > 4 else ""),
                    "created": {
                        "departmentName_inferred": dept_inferred,
                        "roleCode_unmapped": code is None,
                        # the doc provides NO email / Entra id -> required to actually create the user later
                        "email_missing": True, "entraObjectId_missing": True,
                    },
                }
                staff.append(rec)
    staff.sort(key=lambda r: r["sn"])

    # Inferred departments with computed StaffCount; financials NOT in source.
    counts = collections.Counter(s["departmentName"] for s in staff)
    departments = []
    for name in ["Administration & Management", "ICT", "Human Resources",
                 "Business Development", "Relationship & Communications", "Operations",
                 "Agronomy", "Sales", "Finance & Audit", "Quality Assurance", "Crèche/Clinic"]:
        departments.append({
            "name": name,
            "code": re.sub(r"[^A-Z]", "", name.upper())[:6] or name.upper()[:6],
            "description": f"{name} department (grouping inferred from staff positions).",
            "staffCount": counts.get(name, 0),
            "budget": 0, "projectsCount": 0, "percentOfTotal": 0,
            "created": {"department_inferred": True, "staffCount_computed": True,
                        "budget_missing": True, "projectsCount_missing": True, "percentOfTotal_missing": True},
        })
    return staff, departments

# --------------------------------------------------------------------------- #
# XLSX helpers
# --------------------------------------------------------------------------- #
def load_workbook(path):
    z = zipfile.ZipFile(path)
    ss_xml = z.read("xl/sharedStrings.xml").decode("utf-8") if "xl/sharedStrings.xml" in z.namelist() else "<x/>"
    strings = []
    for si in re.findall(r"<si>(.*?)</si>", ss_xml, re.S):
        strings.append(unesc("".join(re.findall(r"<t[^>]*>(.*?)</t>", si, re.S))))
    rels = z.read("xl/_rels/workbook.xml.rels").decode("utf-8")
    relmap = dict(re.findall(r'Id="([^"]*)"[^>]*Target="([^"]*)"', rels))
    wb = z.read("xl/workbook.xml").decode("utf-8")
    sheets = [(unesc(n), rid) for n, rid in
              re.findall(r'<sheet[^>]*name="([^"]*)"[^>]*r:id="([^"]*)"', wb)]
    return z, strings, relmap, sheets

def col_to_num(ref):
    c = re.match(r"[A-Z]+", ref).group(0)
    n = 0
    for ch in c:
        n = n * 26 + (ord(ch) - 64)
    return n

def read_sheet(z, relmap, strings, rid):
    data = z.read("xl/" + relmap[rid]).decode("utf-8")
    out = []
    for r in re.findall(r"<row[^>]*>(.*?)</row>", data, re.S):
        cells = {}
        for ref, attr, inner in re.findall(r'<c r="([A-Z]+\d+)"([^>]*)>(.*?)</c>', r, re.S):
            t = (re.search(r't="([^"]*)"', attr) or [None, "n"])
            t = t.group(1) if hasattr(t, "group") else "n"
            v = re.search(r"<v>(.*?)</v>", inner, re.S)
            if v:
                val = v.group(1)
                if t == "s":
                    val = strings[int(val)]
            else:
                val = "".join(re.findall(r"<t[^>]*>(.*?)</t>", inner, re.S))
            val = unesc(val).strip()
            if val:
                cells[col_to_num(ref)] = val
        out.append(cells)
    return out

EXCEL_EPOCH = datetime.date(1899, 12, 30)
def parse_date(raw, carry):
    """Return (iso_date_or_None, inferred_bool). Handles excel serials, dd/mm/yyyy, ditto."""
    if raw is None:
        return carry, carry is not None
    raw = raw.strip()
    if raw in (",,", '"', "''", "“", "”", "ditto", "-", ""):
        return carry, carry is not None
    if re.fullmatch(r"\d{4,5}", raw):  # excel serial
        try:
            return (EXCEL_EPOCH + datetime.timedelta(days=int(raw))).isoformat(), False
        except Exception:
            return carry, True
    m = re.search(r"(\d{1,2})[/-](\d{1,2})[/-](\d{2,4})", raw)
    if m:
        d, mo, y = int(m.group(1)), int(m.group(2)), int(m.group(3))
        if y < 100:
            y += 2000
        try:
            return datetime.date(y, mo, d).isoformat(), False
        except Exception:
            return carry, True
    return carry, True

FRAC = {"½": 0.5, "¼": 0.25, "¾": 0.75, "⅓": 1/3, "⅔": 2/3}
def parse_qty(raw):
    """Return (number_or_0, unit_or_'', parsed_bool, raw)."""
    if raw is None:
        return 0, "", False, ""
    s = raw.strip()
    if not s or s.upper() in ("N/A", "NA", "NIL", "-"):
        return 0, "", False, raw
    norm = s
    for fr, val in FRAC.items():
        norm = norm.replace(fr, f" {val} ")
    m = re.search(r"(\d{1,3}(?:,\d{3})+|\d+(?:\.\d+)?)", norm)
    num = float(m.group(1).replace(",", "")) if m else 0
    unit_m = re.search(r"[A-Za-z]+", re.sub(r"^\s*\d[\d,\.]*\s*", "", norm))
    unit = unit_m.group(0).lower() if unit_m else ""
    return (num if m else 0), unit, bool(m), raw

# Sheet configuration: (category, default_location, txn_type, layout)
SHEET_CFG = {
    "ELECTRICALS":                       ("Electricals", "Main Store", "Purchase", "std"),
    "PLUMBING MATERIALS":                ("Plumbing Materials", "Main Store", "Purchase", "std"),
    "AGRO- CHEMICALSMATERIALS & SEED":   ("Agro-Chemicals, Materials & Seed", "Main Store", "Purchase", "std"),
    "OTHER ITEMS":                       ("Other Items", "Main Store", "Purchase", "std"),
    "WORKING TOOLS & CONSTRUCTION MA":   ("Working Tools & Construction Materials", "Main Store", "Purchase", "std"),
    "CLINIC":                            ("Clinic", "Main Store", "Purchase", "std"),
    "POULTRY":                           ("Poultry", "Main Store", "Purchase", "std"),
    "CONTRACT ITEM VERIFICATION":        ("Project Supplies", "Main Store", "Purchase", "std"),
    "IBARA PROJECT":                     ("Project Supplies", "Ibara Project", "Purchase", "std"),
    "ITEM FROM LEKKI SHOP":              ("Project Supplies", "Lekki Shop", "Purchase", "std"),
    "ITEMS FROM OWIWI":                  ("Project Supplies", "Owiwi", "Purchase", "std"),
    "ENGR OYENIYI":                      ("Project Supplies", "Engr Oyeniyi", "Purchase", "std"),
    "ITEM RETURNED":                     ("Project Supplies", "Main Store", "Return", "returned"),
    "CONSTRUCTION MATERIALS RECEIVED":   ("Construction Materials", "Main Store", "Purchase", "construction"),
}
CAT_PREFIX = {
    "Electricals": "ELEC", "Plumbing Materials": "PLMB",
    "Agro-Chemicals, Materials & Seed": "AGRO", "Other Items": "OTHR",
    "Working Tools & Construction Materials": "WTCM", "Construction Materials": "CONS",
    "Clinic": "CLIN", "Poultry": "PLTR", "Project Supplies": "PROJ",
}
LOCATION_TYPE = {  # LocationType enum: Warehouse=1, RetailStore=7 (Type is INFERRED)
    "Main Store": ("Warehouse", False), "Ibara Project": ("Warehouse", True),
    "Lekki Shop": ("RetailStore", True), "Owiwi": ("Warehouse", True),
    "Engr Oyeniyi": ("Warehouse", True),
}

def norm_name(n):
    return re.sub(r"\s+", " ", n).strip()

def looks_like_header(name):
    return bool(re.match(r"^(ITEMS?|DATE|S/N|QTY|DELIVERY|TOTAL)\b", name, re.I)) or name.isdigit()

def build_inventory():
    z, strings, relmap, sheets = load_workbook(SRC_XLSX)
    items = {}          # (category, lower-name) -> item dict
    transactions = []
    seq = collections.Counter()

    def get_item(category, name):
        key = (category, name.lower())
        if key not in items:
            seq[category] += 1
            sku = f"{CAT_PREFIX.get(category,'ITEM')}-{seq[category]:04d}"
            items[key] = {
                "sku": sku, "name": norm_name(name), "categoryName": category,
                "unitType": "piece", "quantityOnHand": 0.0,
                "_units": collections.Counter(),
                "created": {"sku_generated": True, "unitType_inferred": True,
                            "costPrice_missing": True, "sellingPrice_missing": True,
                            "minStockLevel_missing": True, "quantityOnHand_summed": True},
            }
        return items[key]

    for name, rid in sheets:
        cfg = SHEET_CFG.get(name)
        if not cfg:
            continue
        category, def_loc, txntype, layout = cfg
        rows = read_sheet(z, relmap, strings, rid)
        carry_date = None

        for cells in rows:
            if not cells:
                continue
            maxc = max(cells)
            row = [cells.get(i, "") for i in range(1, maxc + 1)]

            if layout == "std":
                # DATE | ITEMS | QTY | DISPATCH | RECEIVED BY | COMMENTS
                date_raw = row[0] if len(row) > 0 else ""
                item_name = row[1] if len(row) > 1 else ""
                qty_raw = row[2] if len(row) > 2 else ""
                dispatch = row[3] if len(row) > 3 else ""
                received = row[4] if len(row) > 4 else ""
                comments = row[5] if len(row) > 5 else ""
            elif layout == "returned":
                # S/N | DATE | ITEM | QUANTITY | RECEIVED BY | RETURNED BY
                date_raw = row[1] if len(row) > 1 else ""
                item_name = row[2] if len(row) > 2 else ""
                qty_raw = row[3] if len(row) > 3 else ""
                received = row[4] if len(row) > 4 else ""
                dispatch = row[5] if len(row) > 5 else ""  # returned by
                comments = ""
            elif layout == "construction":
                item_name = row[0] if len(row) > 0 else ""
                if not item_name or looks_like_header(item_name):
                    continue
                it = get_item(category, item_name)
                # first receipt: DELIVERY DATE(1)=B, QTY(1)=C
                d_iso, d_inf = parse_date(row[1] if len(row) > 1 else "", carry_date)
                if d_iso:
                    carry_date = d_iso
                qn, qu, qok, qraw = parse_qty(row[2] if len(row) > 2 else "")
                if qu: it["_units"][qu] += 1
                it["quantityOnHand"] += qn
                transactions.append({
                    "itemSku": it["sku"], "categoryName": category, "locationName": def_loc,
                    "transactionType": txntype, "quantity": qn, "rawQuantity": qraw,
                    "transactionDate": d_iso, "notes": "Initial delivery",
                    "sourceSheet": name,
                    "created": {"quantity_unparseable": (not qok and bool(qraw)),
                                "date_inferred": d_inf, "locationName_defaulted": True}})
                # subsequent combined "date & qty" columns D..G (indices 3..6) -> raw history
                for ci in range(3, min(7, len(row))):
                    cell = row[ci]
                    if not cell or cell == "-":
                        continue
                    cd_iso, cd_inf = parse_date(cell, carry_date)
                    cn, cu, cok, _ = parse_qty(cell)
                    if cu: it["_units"][cu] += 1
                    it["quantityOnHand"] += cn
                    transactions.append({
                        "itemSku": it["sku"], "categoryName": category, "locationName": def_loc,
                        "transactionType": txntype, "quantity": cn, "rawQuantity": cell,
                        "transactionDate": cd_iso, "notes": "Additional delivery (raw): " + cell,
                        "sourceSheet": name,
                        "created": {"quantity_unparseable": (not cok),
                                    "date_inferred": cd_inf, "locationName_defaulted": True}})
                continue
            else:
                continue

            item_name = norm_name(item_name)
            if not item_name or looks_like_header(item_name):
                continue

            it = get_item(category, item_name)
            d_iso, d_inf = parse_date(date_raw, carry_date)
            if d_iso:
                carry_date = d_iso
            qn, qu, qok, qraw = parse_qty(qty_raw)
            if qu:
                it["_units"][qu] += 1
            it["quantityOnHand"] += qn
            note_bits = []
            if qraw: note_bits.append(f"Received: {qraw}")
            if dispatch: note_bits.append(f"Dispatch/By: {dispatch}")
            if received: note_bits.append(f"Received by: {received}")
            if comments and comments not in ("✓", "✔"): note_bits.append(f"Comment: {comments}")
            transactions.append({
                "itemSku": it["sku"], "categoryName": category, "locationName": def_loc,
                "transactionType": txntype, "quantity": qn, "rawQuantity": qraw,
                "transactionDate": d_iso, "notes": " | ".join(note_bits),
                "sourceSheet": name,
                "created": {"quantity_unparseable": (not qok and bool(qraw)),
                            "date_inferred": d_inf, "locationName_defaulted": (def_loc == "Main Store")}})

    # finalize items: choose dominant unit, round qty
    item_list = []
    for it in items.values():
        if it["_units"]:
            it["unitType"] = it["_units"].most_common(1)[0][0]
        del it["_units"]
        it["quantityOnHand"] = round(it["quantityOnHand"], 4)
        item_list.append(it)
    item_list.sort(key=lambda x: x["sku"])

    categories = [{"name": c, "description": f"{c} (from source worksheet).",
                   "created": {"fromSheetName": True}} for c in
                  ["Electricals", "Plumbing Materials", "Agro-Chemicals, Materials & Seed",
                   "Other Items", "Working Tools & Construction Materials", "Construction Materials",
                   "Clinic", "Poultry"]]
    categories.append({"name": "Project Supplies",
                       "description": "Catch-all for items received against projects/shops/returns where the source gives no material category.",
                       "created": {"category_invented_catchall": True}})

    locations = []
    for loc, (ltype, inferred) in LOCATION_TYPE.items():
        locations.append({"name": loc, "type": ltype,
                          "created": {"type_inferred": inferred,
                                      "location_default": (loc == "Main Store")}})
    return categories, locations, item_list, transactions

# --------------------------------------------------------------------------- #
def main():
    print(f"Sources:\n  docx: {SRC_DOCX}\n  xlsx: {SRC_XLSX}\nOutput: {OUT}\n")
    write("roles.json", build_roles())
    staff, departments = build_staff_and_departments()
    write("departments.json", departments)
    write("staff.json", staff)
    categories, locations, items, txns = build_inventory()
    write("categories.json", categories)
    write("locations.json", locations)
    write("items.json", items)
    write("inventory-transactions.json", txns)

    # quick summary for the operator
    unparseable = sum(1 for t in txns if t["created"]["quantity_unparseable"])
    print(f"\nSummary: {len(staff)} staff, {len(departments)} departments, "
          f"{len(categories)} categories, {len(locations)} locations, "
          f"{len(items)} items, {len(txns)} transactions "
          f"({unparseable} with unparseable qty -> 0).")

if __name__ == "__main__":
    main()
