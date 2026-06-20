# Eupepsia / Soilless Farm Lab — Data Import Notes

This document records exactly what was imported into the CRM from the two customer source
documents, and — critically — **every value that was CREATED or INFERRED** because the source did
not provide it. Please review the CREATED/INFERRED items below and correct any that are wrong
before treating this data as authoritative.

## Source documents
1. **CRM Role Allocation Document.docx** → roles, departments (inferred), staff (parked).
2. **SHIPMENTS RECEIVED CARD (1).xlsx** (14 worksheets) → categories, locations, items, receiving history.

## How the data was produced
- `tools/import/extract.py` parses both files and writes auditable staging JSON to
  `CRM.Data/Seeds/Data/Eupepsia-Seed-Data/` (embedded into the `CRM.Data` assembly).
  Every record keeps a `created` / `*_inferred` flag for each generated field, plus a `source`
  (sheet/row) reference. Re-run with `python tools/import/extract.py`.
- Backend seeders load that JSON (idempotent, guarded), all on the **SYSTEM tenant**:
  - Roles → `CRM.Data/Seeds/Seeder.cs`
  - Departments → `CRM.Data/Seeds/OperationalDataSeeder.cs` (also `POST /seed-operational-data`)
  - Inventory → `CRM.Data/Seeds/InventoryDataSeeder.cs` (also `POST /seed-inventory-data`)
  - All three also run automatically from `Seeder.Intialize()` (dev startup / `dotnet run -- --seed`).
- Front-end reusable importers: **Inventory → Items → Import** (live) and **Users → Import Users**
  (preview-only, gated — see G-U1).

## What was loaded (verified against the live database)
| Data | Count | Notes |
|---|---:|---|
| Roles | 6 documented tiers (+ pre-existing STAFF) | SUPER_ADMIN, ADMIN, MANAGER, MANAGER_ACCESS, DEPT_USER, ARTISAN |
| Departments | 11 | inferred from staff positions |
| Categories | 9 | 8 from sheet names + 1 catch-all "Project Supplies" |
| Locations | 5 | Main Store (default) + Ibara Project, Lekki Shop, Owiwi, Engr Oyeniyi |
| Items | 1,857 | de-duplicated master catalogue |
| Inventory transactions | 2,436 | full receiving history (7 are Returns) |
| Staff/users | 0 (47 parked) | deferred — see G-U1 |

Role → permission counts (verified): SUPER_ADMIN 50 (all), ADMIN 45 (all − 5 admin-mgmt),
MANAGER 30, MANAGER_ACCESS 30, DEPT_USER 14, ARTISAN 3, STAFF 8.

---

## CREATED / INFERRED — please review

### Roles
- **G-R1 — "five vs six" tiers.** The doc's prose says "five access tiers" but its table lists
  **six** rows. We created **six** roles (per agreed decision). *Confirm this is intended.*
- **G-R2 — permission sets are CREATED.** The doc describes tiers in prose but lists **no**
  permission codes. The per-role permissions (see counts above) were derived from those
  descriptions. *Review whether each role's access matches intent.*
- **G-R3 — MANAGER (ACCESS) cross-department scope NOT enforced.** The doc says this role is
  "cross-departmental", but the system has no department-scoping mechanism on roles. The role was
  created with the **same permissions as MANAGER**; the cross-dept distinction is currently
  cosmetic. Enforcing it would require a new scoping feature.
- **G-R4 — ADMIN vs SUPER_ADMIN boundary is CREATED.** Per the doc, only SUPER ADMIN onboards
  users / configures settings. So SUPER_ADMIN gets **all** permissions and ADMIN gets all **except**
  these 5 admin-management codes: `admin.users.manage`, `admin.roles.manage`,
  `admin.settings.manage`, `admin.modules.manage`, `admin.menus.manage`. This is a behaviour change
  vs the previous "ADMIN has everything". *Confirm acceptable.*

### Departments
- **G-D1 — the entire department list is INFERRED.** The doc has **no** department master list; it
  only lists each person's `Position`. The 11 departments were created by grouping positions:
  Administration & Management, ICT, Human Resources, Business Development,
  Relationship & Communications, Operations, Agronomy, Sales, Finance & Audit, Quality Assurance,
  Crèche/Clinic. *Review the groupings (see `staff.json` for the per-person mapping).*
- **G-D2 — financials are placeholders.** `Budget`, `ProjectsCount`, `PercentOfTotal` are **not in
  the source** → all seeded as **0**. `StaffCount` was **computed** from the 47-person list
  (Operations 19, Sales 7, Agronomy 4, Relationship & Communications 4, Administration 3,
  Business Development 3, ICT 2, Quality Assurance 2, HR 1, Finance & Audit 1, Crèche/Clinic 1).

### Staff / Users (DEFERRED)
- **G-U1 — no users were created.** A CRM `User` requires a unique **Email** and a Microsoft
  **Entra (Azure AD) object id**; the doc provides **neither**. All 47 staff are parked in
  `staff.json` with their inferred department, mapped role code, position and downliners.
- **Room added for later import:** `User.Position` (nullable) and `User.ManagerId` (nullable
  self-FK) were added (migration `AddUserPositionAndManager`) and exposed on the user DTOs. Once
  emails/Entra ids are supplied, staff can be imported (role from `CRM Role`, `Position`, inferred
  department, and `Downliners` → `ManagerId`). The **Users → Import Users** screen previews a sheet
  but its import is intentionally disabled until those columns and a `POST /users/import` exist.
- Note: the front-end Add-User "Department head" (`headName`) field has **no backend column** and is
  not persisted (pre-existing gap, unrelated to this import).

### Inventory — Categories & Locations
- **G-I4a — "Project Supplies" category is INVENTED.** The 4 project/shop sheets (Ibara, Lekki,
  Owiwi, Engr Oyeniyi), the Contract-Item-Verification sheet and the Item-Returned sheet have **no
  material category**, so their items were placed in a created catch-all **"Project Supplies"**
  (116 items). The other 8 categories come directly from sheet names (normalised).
- **G-I4b — Locations & their Types are INFERRED.** "Main Store" is a **created default** location
  (the category sheets give no location). Location `Type` is inferred: Lekki Shop → RetailStore;
  Main Store / Ibara / Owiwi / Engr Oyeniyi → Warehouse. *Adjust types if known.*

### Inventory — Items
- **G-I1 — all 1,857 SKUs are GENERATED** (`<CAT>-NNNN`, e.g. `ELEC-0001`). The source has no SKUs.
- **G-I2 — UnitType is PARSED/INFERRED** from free-text quantities (e.g. "8 bundles" → `bundle`).
  Defaulted to `piece` when no unit was present.
- **G-I3 — no CostPrice / SellingPrice / MinStockLevel / ReorderQuantity** in the source → left null.
- **G-I6 — items are DE-DUPLICATED** by normalised name within a category; each original receiving
  row became one transaction. Distinct counts per category: Other Items 887, Plumbing 312,
  Agro-Chemicals 252, Electricals 125, Project Supplies 116, Working Tools & Construction 75,
  Construction Materials 60, Poultry 17, Clinic 13.
- **G-I7 — QuantityOnHand is an APPROXIMATE SUM** of each item's parsed receipt quantities
  (affected by G-I2). Treat as an opening figure, not a verified stock count.

### Inventory — Transactions (2,436)
- **G-I5 — dates.** Excel serial dates were converted faithfully. **50** rows used a forward-filled
  ("ditto") date carried from the row above (flagged `date_inferred`). The wide
  "Construction Materials Received" sheet's extra date/qty columns were captured as additional
  transactions with the raw text preserved in `Notes`.
- **G-I2 — quantities.** **2** rows had non-numeric quantities ("N/A") → stored as **0** with the
  raw value kept in `Notes`. All other quantities parsed to numbers.
- **Location defaulting.** **2,324** transactions are on the created default "Main Store"
  (the category sheets carry no location); the remaining 112 are on the named project/shop locations.
- **Transaction type.** All receipts → `Purchase`; the 7 "Item Returned" rows → `Return`.
- Dispatcher / receiver / comment text from each row is preserved in the transaction `Notes`.

---

## Re-running / reset
- Empty + rebuild: `dotnet ef database drop --force …` then `dotnet ef database update …`
  (recreates schema + static seeds), then the seeders run on startup or via the seed endpoints.
- All seeders are idempotent: they guard on existing SYSTEM-tenant rows
  (`IgnoreQueryFilters().AnyAsync(x => x.TenantId == tenantId)`) so re-running does not duplicate.
