Project Overview
The Accounting Ledger System provides fundamental accounting functionalities, allowing users to interact through a user-friendly web interface. It adheres to Clean Architecture principles for maintainability, scalability, and separation of concerns.

Stack
Backend: ASP.NET Core Web API (.NET 8)

Entity Framework Core (for ORM)

MediatR (for CQRS pattern)

AutoMapper (for object mapping)

FluentValidation (for backend validation)

SQL Server (Database) - Utilizing Stored Procedures for Journal Entry creation and Trial Balance retrieval.

Frontend: ASP.NET Core Razor Pages

Tailwind CSS (for styling - via CDN or local setup if configured)

jQuery and jQuery Validation Unobtrusive (for client-side form validation)

Goal
The primary goal of this project is to create a modular accounting module that enables users to:

Efficiently manage a chart of accounts.

Accurately record financial transactions as double-entry journal entries.

Generate and view a simplified trial balance report.

Interact with the system via a responsive web application.

Features
Backend (ASP.NET Core Web API)
Solution Setup: Implemented using Clean Architecture principles, separating domain, application, infrastructure, and presentation layers.

Dependencies: Configured with EF Core, AutoMapper, MediatR, and FluentValidation.

Database Integration:

SQL Server connection and EF Core migrations setup.

Stored Procedures Used:

sp_InsertJournalEntry: For creating new journal entries, ensuring debit/credit balance.

sp_GetTrialBalance: For calculating and retrieving account net balances.

Entities:

Account: Represents an accounting account with Id, Name, and Type (Asset, Liability, Equity, Revenue, Expense).

JournalEntry: Represents a single journal entry with Id, Date, Description, and a collection of JournalEntryLines.

JournalEntryLine: Represents a debit or credit line item within a journal entry, linking to an Account and specifying Debit or Credit amounts.

Core APIs (via MediatR commands/queries):

CreateAccountCommand: Adds a new account.

GetAccountsQuery: Retrieves all accounts.

CreateJournalEntryCommand: Creates a new journal entry. Includes validation to ensure total debits equal total credits.

GetJournalEntriesQuery: Retrieves journal entries (with optional date filtering).

GetTrialBalanceQuery: Calculates and returns net balances by account.

Frontend (Razor Pages)
Responsive UI: Designed to be usable across various devices.

Pages:

Accounts Page:

Allows viewing existing accounts.

Provides a form to add new accounts with Name and Type.

Journal Entry Page:

Enables creation of journal entries with a Date and Description.

Supports dynamic addition/removal of journal entry lines.

Auto-calculates and displays total debit and credit amounts.

Prevents submission unless total debits exactly equal total credits and total is greater than zero.

View Journal Entries Page:

Displays a table of all recorded journal entries, including their lines.

Includes a date filter (Start Date, End Date).

Trial Balance Page:

Lists all accounts with their calculated net debit or credit balance.

Verifies if the overall trial balance is balanced.

Filtering: Date filtering implemented for journal entries.

Validation:

Frontend form validation (e.g., preventing empty entries, ensuring debit/credit balance on journal entry lines).

Backend validation using FluentValidation for all commands.

Error Handling + UI Feedback:

Display of validation and API errors clearly on relevant pages.

Success messages using TempData for operations like creating accounts or journal entries.

Basic client-side feedback for journal entry balancing.

Setup & Run Instructions
To get this project up and running on your local machine, follow these steps:

Prerequisites:

.NET 8 SDK

SQL Server (Express Edition or Developer Edition is sufficient)

SQL Server Management Studio (SSMS) (Optional, for database inspection)

Clone the Repository:

git clone https://github.com/julkarnimece/AccountingLedger.git
cd AccountingLedger 
Configure Database Connection:

Open AccountingLedger.Web/appsettings.json.

Update the DefaultConnection string to point to your SQL Server instance.
Example for LocalDB (default, often works out-of-the-box):

"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AccountingLedgerDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}

If you're using a different SQL Server instance, update Server accordingly.

Run EF Core Migrations & Apply Database Schema:

Navigate to the AccountingLedger.Web directory in your terminal:

cd AccountingLedger.Web

Apply migrations to create the database schema:

dotnet ef migrations add Initial --project ..\AccountingLedger.Infrastructure --s .
dotnet ef database update --project ..\AccountingLedger.Infrastructure --s .

This command will also seed initial data (accounts and a couple of journal entries)  in ApplicationDbContextSeed.cs and Program.cs.

Create Stored Procedures and Table Type:

Manually execute the SQL scripts for the JournalEntryLineType user-defined table type, sp_InsertJournalEntry, and sp_GetTrialBalance against your database. You can use SSMS or any SQL client for this.

JournalEntryLineType.sql (define the TVP)

sp_InsertJournalEntry.sql

sp_GetTrialBalance.sql


Run the Application:

From the AccountingLedger.Web directory:

dotnet run

The application should start, and you'll see a URL (e.g., http://localhost:5296/). Open this URL in your browser.
