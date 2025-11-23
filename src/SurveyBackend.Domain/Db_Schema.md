# Survey System Database Schema & Domain Rules

This document outlines the database schema, relationships, and enums for the Survey System.
**Architecture:** Clean Architecture + Domain-Driven Design (DDD).
**Technology Stack:** .NET Core 9, Entity Framework Core (Code-First).

## 1. Enums

### AccessType
Defines who can access the survey.
- `Internal` (0): Requires LDAP Authentication (Company Employees).
- `Public` (1): Open to everyone (No Login required, tracked via Cookie/Session).

### QuestionType
Defines the UI and data storage logic for a question.
- `SingleSelect` (0): Radio buttons (uses AnswerOption).
- `MultiSelect` (1): Checkboxes (uses AnswerOption).
- `OpenText` (2): TextArea (uses Answer.TextValue).

---

## 2. Domain Entities (Aggregates & Roots)

### Survey (Aggregate Root)
Represents the survey definition.
*Behaviors:* `Publish()`, `AddQuestion()`.
- `Id` (Guid, PK)
- `Title` (string, MaxLength: 200, Required)
- `Description` (string, MaxLength: 2000, Nullable) - HTML/Markdown supported.
- `CreatedBy` (string, MaxLength: 100, Required) - Username/ID of the creator.
- `CreatedAt` (DateTimeOffset, Required) - UTC.
- `IsActive` (bool, Required) - Default: false.
- `AccessType` (Enum: AccessType, Required)
- `StartDate` (DateTimeOffset, Nullable)
- `EndDate` (DateTimeOffset, Nullable)
- **Navigation:** `ICollection<Question> Questions`

### Question (Entity)
Represents a question within a survey.
*Behaviors:* `AddOption()`.
- `Id` (Guid, PK)
- `SurveyId` (Guid, FK -> Survey)
- `Text` (string, MaxLength: 1000, Required)
- `Description` (string, MaxLength: 500, Nullable)
- `Order` (int, Required)
- `Type` (Enum: QuestionType, Required)
- `IsRequired` (bool, Required)
- **Navigation:** `ICollection<QuestionOption> Options`

### QuestionOption (Entity)
Represents choices for SingleSelect/MultiSelect questions.
- `Id` (Guid, PK)
- `QuestionId` (Guid, FK -> Question)
- `Text` (string, MaxLength: 500, Required)
- `Order` (int, Required)
- `Value` (int, Nullable) - Optional score value for the option (e.g. Correct answer = 10 points).
- **Navigation:** `ICollection<DependentQuestion> DependentQuestions`

### DependentQuestion (Entity)
Handles branching logic. "If ParentOption is selected, show ChildQuestion".
- `Id` (Guid, PK)
- `ParentQuestionOptionId` (Guid, FK -> QuestionOption) - The trigger.
- `ChildQuestionId` (Guid, FK -> Question) - The action (question to show).
*Note:* The order of child questions is determined by `Question.Order`.

### Participant (Entity)
Represents the user taking the survey.
- `Id` (Guid, PK)
- `ExternalId` (Guid, Nullable) - Unique Cookie ID for Public users.
- `LdapUsername` (string, MaxLength: 100, Nullable) - For Internal users.
- `CreatedAt` (DateTimeOffset, Required)

### Participation (Aggregate Root)
Represents a single session of a user taking a survey.
*Behaviors:* `Start()`, `Complete()`, `AddAnswer()`.
- `Id` (Guid, PK)
- `SurveyId` (Guid, FK -> Survey)
- `ParticipantId` (Guid, FK -> Participant)
- `StartedAt` (DateTimeOffset, Required)
- `CompletedAt` (DateTimeOffset, Nullable) - If null, survey is incomplete.
- `IpAddress` (string, MaxLength: 50, Nullable)
- **Navigation:** `ICollection<Answer> Answers`

### Answer (Entity)
Represents the user's response to a specific question.
- `Id` (Guid, PK)
- `ParticipationId` (Guid, FK -> Participation)
- `QuestionId` (Guid, FK -> Question)
- `TextValue` (string, Nullable) - Only used for `OpenText` questions.
- **Navigation:** `ICollection<AnswerOption> SelectedOptions`

### AnswerOption (Entity)
Junction table for selected choices in SingleSelect/MultiSelect questions.
- `Id` (Guid, PK)
- `AnswerId` (Guid, FK -> Answer)
- `QuestionOptionId` (Guid, FK -> QuestionOption)

---

## 3. Persistence Rules (EF Core Configurations)

1.  **Delete Behavior:**
    - Deleting a `Survey` -> Cascades to `Questions`.
    - Deleting a `Question` -> Cascades to `Options` and `Answers`.
    - Deleting a `Participation` -> Cascades to `Answers`.
2.  **Logic Constraint:**
    - `DependentQuestion` relationships should use `Restrict` on delete for the Parent to avoid cycles.
3.  **Querying:**
    - Always use `AsSplitQuery()` when fetching deep hierarchies (Survey -> Questions -> Options -> Dependent -> Child).