<h1>**#Mini Account Management System**</h1>
MiniAccountManagementSystem is a lightweight web application built with ASP.NET Core Razor Pages, designed to manage a chart of accounts and vouchers in a hierarchical structure. This project provides a user-friendly interface for creating, viewing, editing, and deleting account and voucher data, with role-based access control and data export capabilities.

<h3>**Overview**</h3>
This application serves as a basic accounting management system, allowing users to maintain a chart of accounts with parent-child relationships and record financial vouchers. It includes a flat table view and a hierarchical tree view for accounts, with restricted access for different user roles.

<h4>Features</h4>
**1. Account Management :**
Chart of Accounts: Display a flat list of accounts in a table, showing Account ID, Name, Type, Balance, Created Date, and Parent Account ID.
Hierarchical Account Tree: View accounts in a nested tree structure, reflecting parent-child relationships with indentation based on hierarchy levels.
Create Account: Add new accounts with details like Name, Type, Balance, and optional Parent Account ID (restricted to Admin users).
Edit Account: Modify existing account details (restricted to Admin users).
Delete Account: Remove accounts and their associated data (restricted to Admin users).
Export to Excel: Export the flat account list to an Excel file (AccountsData.xlsx) for offline analysis (restricted to Admin users).
**2. Voucher Management:**
Voucher Listing: View a list of vouchers with their details, including Voucher ID, Type, Reference Number, Date, and Created By.
Create Voucher: Record new vouchers with multiple entries linking to accounts (e.g., Debit and Credit amounts), including dynamic entry addition via JavaScript (restricted to Admin users).
Edit Voucher: Update existing voucher details and entries (restricted to Admin users).
Delete Voucher: Remove vouchers and their entries (restricted to Admin users).
**3. Role-Based Access Control:**
User Roles: Supports two roles - Admin and Viewer.
Admin: Full access to create, edit, delete, and export data.
Viewer: Read-only access to view account and voucher details; Create, Edit, and Delete buttons are hidden.
Authorization: Implemented using ASP.NET Core [Authorize] and [Authorize(Roles = "Admin")] attributes, with conditional rendering in views using User.IsInRole("Admin").
**4. Data Persistence:**
Database: Uses SQL Server with stored procedures (sp_ManageChartofAccounts, sp_SaveVoucher, etc.) for CRUD operations and hierarchical data retrieval.
Data Access: Custom DbAccess class handles database interactions with parameterized queries.
**5. User Interface:**
Responsive Design: Built with Bootstrap for a clean, professional layout across devices.
Navigation: Integrated with a layout page (_Layout.cshtml) for consistent navigation.
Validation: Client-side and server-side validation for form inputs.
**6. Additional Features:**
Manage Roles: A page to assign or remove roles (e.g., Viewer, Admin) for users, with inline removal confirmation.
Dynamic Forms: JavaScript enables dynamic addition and removal of voucher entries.
