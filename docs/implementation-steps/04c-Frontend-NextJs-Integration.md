# Phase 4c — Frontend Integration: Next.js Authorization

> **Goal**: Configure the Next.js application to respect the localized permissions and roles defined in the CRM backend.

---

## Step 1 — Permission Awareness

The Next.js frontend needs to know what the user is allowed to do. It should fetch this information immediately after a successful login.

### 1.1 Fetch User Profile

Call the `GET /api/v1/users/me` endpoint. The response includes a `Permissions` list:

```json
{
  "id": "...",
  "email": "user@example.com",
  "permissions": [
    "inventory.items.view",
    "inventory.items.create",
    "admin.users.manage"
  ]
}
```

### 1.2 Access Control Store (Zustand/Context)

Store these permissions in a global state (e.g., using Zustand or React Context) to make them easily accessible throughout the UI.

```typescript
const useAuthStore = create((set) => ({
  permissions: [],
  setPermissions: (perms) => set({ permissions: perms }),
  hasPermission: (perm) => get().permissions.includes(perm),
}));
```

---

## Step 2 — UI-Level Authorization

Use a helper component or hook to wrap UI elements that require specific permissions.

### 2.1 The `Can` Component

Create a wrapper component to conditionally render children:

```tsx
export function Can({ I, children }: { I: string; children: React.ReactNode }) {
  const { hasPermission } = useAuthStore();

  if (!hasPermission(I)) return null;

  return <>{children}</>;
}

// Usage:
<Can I="inventory.items.create">
  <Button>Add New Item</Button>
</Can>;
```

---

## Step 3 — Navigation & Menus

Instead of hardcoding the sidebar, the frontend should fetch allowed navigation items from the API.

### 3.1 Fetch Allowed Menus

Call `GET /api/v1/menus/my-menus`. This endpoint recursively returns only the menu items the user has permission to see based on the local database roles and permissions.

### 3.2 Dynamic Sidebar

Iterate through the returned menu tree to render your navigation links.

```tsx
const { data: navigation } = useQuery(["menus"], fetchMyMenus);

return (
  <nav>
    {navigation.map((item) => (
      <NavLink
        key={item.id}
        href={item.route}
        label={item.label}
        icon={item.icon}
      />
    ))}
  </nav>
);
```

---

## Step 4 — Verification

- [ ] Log in as a user with the `Administrator` local role. Verify all sidebar items and "Add" buttons are visible.
- [ ] Log in as a user with a `Viewer` role (limited permissions). Verify restricted sidebar items and buttons are automatically hidden.
- [ ] Attempt to manually navigate to a restricted URL (Expected: Backend should still reject the API calls with `403 Forbidden`).

---

→ [Phase 5b — Local Application Authorization](file:///c:/1%20-Tahyour/8%20-%20Projects/CRM/CRM%20-%20BE/docs/implementation-steps/05b-Auth-Local-Authorization.md)
