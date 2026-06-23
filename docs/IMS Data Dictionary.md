Here is a **comprehensive data dictionary** for the multi-tenant agriculture (farming) inventory management system, built on Azure SQL Database with C# backend and Next.js frontend. The design emphasizes:

- **Multi-tenancy** (tenant_id on most tables for data isolation)
- **Agriculture-specific needs** (batch/lot traceability, expiration/perishability, units/variability, storage conditions, multi-location)
- **Inventory accuracy** (quantity in decimal for kg/l/ha precision, location-level stock)
- **Auditability** & compliance (timestamps, soft deletes, audit logs)
- **Integrations** readiness (e-commerce sync fields, barcode/scan support)

All tables include standard auditing columns where relevant:

- `created_at` DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
- `created_by` INT NULL (FK → Users)
- `updated_at` DATETIME2 NULL
- `updated_by` INT NULL (FK → Users)
- `is_deleted` BIT NOT NULL DEFAULT 0 (soft delete flag)
- `tenant_id` INT NOT NULL (FK → Tenants, enforced via row-level security or application logic)

### 1. Tenants

Represents each farm/agribusiness using the system (multi-tenant root).

| Column Name         | Data Type     | Nullable | Description / Business Rule                                        | Constraints / Notes          |
| ------------------- | ------------- | -------- | ------------------------------------------------------------------ | ---------------------------- |
| id                  | INT           | No       | Primary key – unique tenant identifier                             | PK, IDENTITY(1,1)            |
| name                | NVARCHAR(150) | No       | Official farm/business name                                        | NOT NULL, indexed            |
| code                | NVARCHAR(20)  | Yes      | Short unique code (e.g., "FARM-NG-001")                            | UNIQUE per tenant (optional) |
| registration_number | NVARCHAR(50)  | Yes      | CAC / business registration number (Nigeria context)               |                              |
| industry_type       | NVARCHAR(50)  | Yes      | e.g., "Crop Farming", "Mixed Farming", "Livestock", "Horticulture" |                              |
| address             | NVARCHAR(500) | Yes      | Physical / primary farm address                                    |                              |
| contact_email       | NVARCHAR(150) | Yes      | Primary contact email                                              |                              |
| contact_phone       | NVARCHAR(30)  | Yes      | Primary phone number                                               |                              |
| subscription_plan   | NVARCHAR(50)  | Yes      | e.g., "Basic", "Pro", "Enterprise" (for future billing)            |                              |
| subscription_status | NVARCHAR(20)  | Yes      | Active, Trial, Expired, Suspended                                  |                              |
| trial_end_date      | DATE          | Yes      | End date if on trial                                               |                              |

### 2. Users

| Column Name   | Data Type     | Nullable | Description / Business Rule                                           | Constraints / Notes              |
| ------------- | ------------- | -------- | --------------------------------------------------------------------- | -------------------------------- |
| id            | INT           | No       | PK – user identifier                                                  | PK, IDENTITY                     |
| tenant_id     | INT           | No       | Owner farm/tenant                                                     | FK → Tenants, NOT NULL           |
| username      | NVARCHAR(100) | No       | Unique login name (per tenant)                                        | UNIQUE (tenant_id, username)     |
| email         | NVARCHAR(150) | No       | Login email                                                           | UNIQUE (tenant_id, email)        |
| password_hash | NVARCHAR(256) | No       | Hashed password (bcrypt / Argon2)                                     |                                  |
| first_name    | NVARCHAR(100) | Yes      |                                                                       |                                  |
| last_name     | NVARCHAR(100) | Yes      |                                                                       |                                  |
| role          | NVARCHAR(50)  | No       | Enum-like: Admin, Manager, Supervisor, Staff, FieldWorker, Accountant | CHECK constraint or lookup table |
| phone         | NVARCHAR(30)  | Yes      |                                                                       |                                  |
| is_active     | BIT           | No       | Default 1                                                             |                                  |
| last_login    | DATETIME2     | Yes      | For security monitoring                                               |                                  |

### 3. Categories

Product/service grouping (tenant-specific).

| Column Name | Data Type     | Nullable | Description                                                                                    | Constraints              |
| ----------- | ------------- | -------- | ---------------------------------------------------------------------------------------------- | ------------------------ |
| id          | INT           | No       | PK                                                                                             | PK                       |
| tenant_id   | INT           | No       |                                                                                                | FK → Tenants             |
| name        | NVARCHAR(100) | No       | e.g., "Seeds", "Fertilizers", "Pesticides", "Equipment", "Harvested Produce", "Livestock Feed" | UNIQUE (tenant_id, name) |
| parent_id   | INT           | Yes      | Self-referencing for hierarchy (e.g., "Herbicides" under "Pesticides")                         | FK → Categories (self)   |
| description | NVARCHAR(500) | Yes      |                                                                                                |                          |

### 4. Suppliers

| Column Name        | Data Type     | Nullable | Description             | Constraints  |
| ------------------ | ------------- | -------- | ----------------------- | ------------ |
| id                 | INT           | No       | PK                      | PK           |
| tenant_id          | INT           | No       |                         | FK → Tenants |
| name               | NVARCHAR(150) | No       | Company / supplier name |              |
| supplier_code      | NVARCHAR(30)  | Yes      | Internal reference code |              |
| contact_person     | NVARCHAR(100) | Yes      |                         |              |
| email              | NVARCHAR(150) | Yes      |                         |              |
| phone              | NVARCHAR(30)  | Yes      |                         |              |
| address            | NVARCHAR(500) | Yes      |                         |              |
| tax_id             | NVARCHAR(50)  | Yes      | VAT / TIN number        |              |
| payment_terms_days | INT           | Yes      | Default credit days     |              |

### 5. Locations

Multi-location support (farms, warehouses, cold rooms, fields as storage points).

| Column Name         | Data Type     | Nullable | Description                                                      | Constraints  |
| ------------------- | ------------- | -------- | ---------------------------------------------------------------- | ------------ |
| id                  | INT           | No       | PK                                                               | PK           |
| tenant_id           | INT           | No       |                                                                  | FK → Tenants |
| name                | NVARCHAR(150) | No       | e.g., "Main Farm Warehouse", "Cold Room A", "Field B Silo"       |              |
| code                | NVARCHAR(30)  | Yes      | Short code                                                       |              |
| type                | NVARCHAR(50)  | No       | Enum: Warehouse, ColdStorage, OpenField, Silo, PackingArea, etc. |              |
| address             | NVARCHAR(500) | Yes      | Full address if different from farm                              |              |
| latitude            | DECIMAL(10,8) | Yes      | GPS coordinate                                                   |              |
| longitude           | DECIMAL(11,8) | Yes      | GPS coordinate                                                   |              |
| capacity            | DECIMAL(18,4) | Yes      | Max capacity (in primary unit, e.g., kg / m³)                    |              |
| temperature_control | BIT           | Yes      | Whether temperature-controlled                                   | Default 0    |

### 6. Items (Core Inventory Master)

| Column Name        | Data Type     | Nullable | Description / Agriculture Focus                                                  | Constraints / Notes     |
| ------------------ | ------------- | -------- | -------------------------------------------------------------------------------- | ----------------------- |
| id                 | INT           | No       | PK                                                                               | PK                      |
| tenant_id          | INT           | No       |                                                                                  | FK → Tenants            |
| sku                | NVARCHAR(50)  | No       | Unique stock-keeping unit (per tenant)                                           | UNIQUE (tenant_id, sku) |
| name               | NVARCHAR(200) | No       | Item name (e.g., "Maize Seed - Hybrid DK8181")                                   | Indexed                 |
| category_id        | INT           | Yes      |                                                                                  | FK → Categories         |
| supplier_id        | INT           | Yes      | Preferred / default supplier                                                     | FK → Suppliers          |
| description        | NVARCHAR(MAX) | Yes      | Detailed specs                                                                   |                         |
| unit_type          | NVARCHAR(30)  | No       | Primary unit: kg, liter, bag, piece, tonne, bunch, crate, etc.                   |                         |
| barcode            | NVARCHAR(100) | Yes      | UPC / EAN / GS1 barcode for scanning                                             | Indexed for fast lookup |
| batch_tracked      | BIT           | No       | Whether batch/lot tracking is required (mandatory for seeds, chemicals, produce) | Default 0               |
| expiry_tracked     | BIT           | No       | Whether expiration date matters (fertilizers, seeds, produce)                    | Default 0               |
| min_stock_level    | DECIMAL(18,4) | Yes      | Reorder threshold                                                                |                         |
| reorder_quantity   | DECIMAL(18,4) | Yes      | Suggested order quantity                                                         |                         |
| cost_price         | DECIMAL(18,4) | Yes      | Latest / average purchase price                                                  |                         |
| selling_price      | DECIMAL(18,4) | Yes      | Default selling price                                                            |                         |
| image_url          | NVARCHAR(500) | Yes      | Azure Blob Storage URL                                                           |                         |
| storage_conditions | NVARCHAR(500) | Yes      | e.g., "Cool dry place < 25°C", "Refrigerated 2-8°C"                              |                         |

### 7. Item_Locations (Stock per location)

| Column Name | Data Type     | Nullable | Description                             | Constraints    |
| ----------- | ------------- | -------- | --------------------------------------- | -------------- |
| id          | INT           | No       | PK                                      | PK             |
| item_id     | INT           | No       |                                         | FK → Items     |
| location_id | INT           | No       |                                         | FK → Locations |
| quantity    | DECIMAL(18,4) | No       | Current physical stock at this location | Default 0      |
| reserved    | DECIMAL(18,4) | Yes      | Stock reserved for orders / production  | Default 0      |
| damaged     | DECIMAL(18,4) | Yes      | Damaged / unusable stock                | Default 0      |

### 8. Batches (for traceability – used when batch_tracked = 1)

| Column Name       | Data Type     | Nullable | Description                                        | Constraints                    |
| ----------------- | ------------- | -------- | -------------------------------------------------- | ------------------------------ |
| id                | INT           | No       | PK                                                 | PK                             |
| item_id           | INT           | No       |                                                    | FK → Items                     |
| batch_number      | NVARCHAR(100) | No       | Supplier batch / lot number or internal generated  | UNIQUE (item_id, batch_number) |
| manufacture_date  | DATE          | Yes      |                                                    |                                |
| expiry_date       | DATE          | Yes      | Critical for perishables & regulated items         | Indexed                        |
| received_date     | DATE          | Yes      | When batch entered inventory                       |                                |
| initial_quantity  | DECIMAL(18,4) | Yes      | Quantity when received                             |                                |
| current_quantity  | DECIMAL(18,4) | No       | Remaining after sales/usage (updated via triggers) |                                |
| supplier_batch_id | NVARCHAR(100) | Yes      | Supplier's own batch code                          |                                |

### 9. Purchase_Orders + Purchase_Order_Items

(Use standard PO structure with added agriculture fields: expected_delivery_date, quality_notes, etc.)

### 10. Sales_Orders + Sales_Order_Items

(Add ecom_order_id NVARCHAR(100), delivery_location_id, harvest_batch_id for traceability)

### 11. Inventory_Transactions (movement audit trail)

Tracks every addition, deduction, transfer, adjustment.

| Column Name      | Data Type     | Nullable | Description                                                               | Notes          |
| ---------------- | ------------- | -------- | ------------------------------------------------------------------------- | -------------- |
| id               | BIGINT        | No       | PK (high volume)                                                          | IDENTITY       |
| tenant_id        | INT           | No       |                                                                           | FK             |
| item_id          | INT           | No       |                                                                           | FK             |
| batch_id         | INT           | Yes      | If batch-tracked                                                          | FK → Batches   |
| location_id      | INT           | Yes      | From/to location (for transfers)                                          | FK → Locations |
| transaction_type | NVARCHAR(30)  | No       | Purchase, Sale, TransferIn, TransferOut, Adjustment, Damage, Return, etc. |                |
| quantity         | DECIMAL(18,4) | No       | Positive = in, Negative = out                                             |                |
| reference_id     | INT           | Yes      | PO id, SO id, Adjustment id, etc.                                         |                |
| notes            | NVARCHAR(500) | Yes      | e.g., "Damaged during transport"                                          |                |
| transaction_date | DATETIME2     | No       | When movement occurred                                                    | Indexed        |

### 12. Audit_Logs

(Captures create/update/delete on critical entities)

This dictionary provides a robust, production-ready foundation. It balances normalization, performance (indexes on frequent filters: tenant_id + sku + expiry_date + location_id), and agriculture compliance needs.

To finalize or expand:

- Do you want to include livestock-specific tables (animals, herds, health records)?
- Should we add crop/field management entities (fields, planting cycles, yield tracking)?
- Do you prefer certain tables to use schema-per-tenant isolation instead of tenant_id filtering?
- Any mandatory regulatory fields (e.g., NAFDAC registration for chemicals in Nigeria)?

Let me know your thoughts on these points so we can refine it accurately!
