# HumbleBookCatalog
## MVP Specification

**Document type:** Product & Technical Specification  
**Version:** 1.0  
**Status:** Approved for MVP development  
**Application type:** .NET Console Application  
**Target framework:** .NET 10  
**Language:** C#

---

## 1. Project Overview

HumbleBookCatalog is a C# console application designed to extract book information from Humble Bundle library HTML content, enrich the extracted information using external bibliographic APIs, and export the resulting catalog.

The application must support both complete HTML documents and partial HTML fragments.

The primary goal is to create a structured, reusable catalog of purchased e-books without requiring the user to download the actual book files.

## 2. MVP Goals

The application must provide the following capabilities:

1. Import book information from HTML files.
2. Support complete HTML documents and partial HTML fragments.
3. Extract book titles and publishers.
4. Handle missing or incomplete book information gracefully.
5. Automatically search for additional book metadata.
6. Retrieve authors, ISBN identifiers, publication dates, and descriptions where available.
7. Export the resulting catalog as CSV or JSON.
8. Provide meaningful error messages and processing reports.

## 3. Input Requirements

### 3.1 Supported Input

The application accepts a local HTML file selected by the user.

Two input structures must be supported:

**A. Complete HTML document**

A complete document containing HTML, HEAD, BODY, and other elements.

**B. HTML fragment**

A partial HTML structure containing one or more book entries, without requiring HTML, HEAD, or BODY elements.

For example:

```html
<div class="book-item">
    <div class="book-title">
        Fundamentals of Software Architecture
    </div>
    <div class="publisher">
        O'Reilly
    </div>
</div>
```

### 3.2 HTML Parsing Strategy

The parser must not depend exclusively on one fixed HTML structure.

Requirements:

- Parse complete HTML documents and fragments.
- Ignore irrelevant HTML elements.
- Identify potential book entries.
- Support multiple book entries in a single document.
- Handle nested elements.
- Handle whitespace and HTML entities.
- Avoid duplicate entries caused by nested or repeated elements.

The parser should use AngleSharp for HTML parsing.

### 3.3 Extraction Rules

Each detected book entry should provide:

| Field | Requirement |
|---|---|
| Title | Required |
| Publisher | Optional |
| Authors | Optional |
| ISBN-10 | Optional |
| ISBN-13 | Optional |

Title and publisher extraction should initially use configurable CSS selectors for known Humble Bundle structures.

A fallback detection strategy should be used when the expected selectors are unavailable.

The application must not assume that every HTML element containing text represents a book.

### 3.4 Input Validation and Error Handling

The application must detect and report:

- Empty HTML files.
- Invalid or unreadable files.
- HTML documents containing no recognizable book entries.
- Book entries without titles.
- Book entries without publishers.
- Unsupported or unrelated HTML content.
- Duplicate book entries.
- Partially readable HTML documents.

Missing publishers must not automatically invalidate an otherwise recognizable book entry.

A book without a title must not be exported as a valid catalog entry.

The application should distinguish between fatal input errors and non-fatal extraction warnings.

## 4. Metadata Enrichment

### 4.1 Objective

The application should enrich extracted book information using external bibliographic APIs.

The primary metadata providers are:

1. Google Books API.
2. Open Library API.

### 4.2 Search Strategy

The application should:

1. Search using the extracted book title.
2. Include the publisher in the search query when available.
3. Compare returned titles and publishers with the original information.
4. Retrieve authors, ISBN-10, ISBN-13, publication date, and description.
5. Use Open Library as a fallback provider when appropriate.
6. Preserve the original extracted information.

ISBN is not required for the initial search because the source HTML may not contain ISBN identifiers.

### 4.3 Automatic Matching

Metadata enrichment runs automatically.

However, automatic matching must not mean accepting every API result without validation.

The application should classify results:

| Status | Description |
|---|---|
| Pending | Metadata lookup has not started. |
| Found | A potential match was found. |
| Verified | The match passed the application's validation criteria. |
| MultipleMatches | Several possible matches were found. |
| NotFound | No suitable result was found. |

Uncertain matches must not overwrite the original book information.

The MVP does not require an interactive manual verification interface. Uncertain matches must be clearly identified in the exported catalog.

### 4.4 API Resilience

The application must handle:

- HTTP errors.
- Network failures.
- API rate limits.
- Missing metadata fields.
- Invalid API responses.
- Timeouts.

A failed metadata lookup must not prevent the export of successfully extracted books.

## 5. Data Model

The primary domain entity is `Book`.

Proposed properties:

| Property | Type | Description |
|---|---|---|
| Title | string | Original book title |
| Authors | List<string> | Book authors |
| Publisher | string? | Original or enriched publisher |
| ISBN10 | string? | ISBN-10 identifier |
| ISBN13 | string? | ISBN-13 identifier |
| PublishedDate | string? | Publication date |
| Description | string? | Book description |
| MetadataSource | string? | Metadata provider |
| MatchStatus | enum | Metadata matching status |

The original extracted title and publisher should be preserved separately from enriched metadata when necessary.

ISBN identifiers must be stored as strings, never numeric values.

## 6. Export Requirements

The application must support two export formats.

The user must be able to choose either format.

### 6.1 CSV Export

The CSV file should contain:

- Title
- Authors
- Publisher
- ISBN10
- ISBN13
- PublishedDate
- Description
- MetadataSource
- MatchStatus

Requirements:

- UTF-8 encoding.
- Proper escaping of delimiters and quotation marks.
- Multiple authors represented in a consistent format.
- ISBN values preserved as text.
- Compatible with Microsoft Excel and LibreOffice.

### 6.2 JSON Export

The JSON file should contain an array of book objects inside a root catalog object.

Requirements:

- Valid JSON.
- UTF-8 encoding.
- Consistent property naming.
- Null values for unavailable optional metadata.
- Multiple authors represented as an array.
- Indented formatting for readability.

### 6.3 Output Location

The user should be able to specify the output directory.

A default output directory should be provided.

Suggested filenames:

- `humble_book_catalog.csv`
- `humble_book_catalog.json`

## 7. Console User Interface

The MVP uses a simple menu-driven console interface.

Example:

```text
========================================
          HUMBLE BOOK CATALOG
========================================

1. Import HTML file
2. Enrich book metadata
3. Export catalog
0. Exit

Select an option:
```

Export menu:

```text
========================================
             EXPORT CATALOG
========================================

1. Export as CSV
2. Export as JSON

Select format:
```

The application must provide clear feedback about:

- Successfully imported books.
- Entries skipped during extraction.
- Metadata lookup results.
- API failures.
- Export completion.
- Output file location.

## 8. Technical Architecture

Proposed project structure:

```text
HumbleBookCatalog/
│
├── src/
│   └── HumbleBookCatalog/
│       │
│       ├── Models/
│       │   ├── Book.cs
│       │   └── BookMetadata.cs
│       │
│       ├── Import/
│       │   ├── IBookImporter.cs
│       │   └── HumbleHtmlParser.cs
│       │
│       ├── Metadata/
│       │   ├── IBookMetadataProvider.cs
│       │   ├── GoogleBooksProvider.cs
│       │   └── OpenLibraryProvider.cs
│       │
│       ├── Export/
│       │   ├── ICatalogExporter.cs
│       │   ├── CsvExporter.cs
│       │   └── JsonExporter.cs
│       │
│       ├── Services/
│       │   └── CatalogService.cs
│       │
│       └── Program.cs
│
├── tests/
│   └── HumbleBookCatalog.Tests/
│
├── data/
│   ├── input/
│   └── output/
│
├── docs/
│
└── README.md
```

### 8.1 Technical Dependencies

| Component | Technology |
|---|---|
| Runtime | .NET 10 |
| HTML parsing | AngleSharp |
| HTTP communication | HttpClient |
| JSON serialization | System.Text.Json |
| CSV serialization | CsvHelper |
| Testing | xUnit |

### 8.2 Architectural Principles

- Separation of responsibilities.
- Dependency inversion through interfaces where appropriate.
- Asynchronous HTTP operations.
- Dependency injection for external services.
- Testable parsing and export components.
- No unnecessary architectural complexity.

Clean Architecture and CQRS are explicitly outside the MVP scope.

## 9. Testing Requirements

The application must include automated tests for the following scenarios.

### HTML Parsing

1. Complete HTML document containing multiple books.
2. HTML fragment containing one book.
3. HTML fragment containing multiple books.
4. Book entry without a publisher.
5. Book entry without a title.
6. Empty HTML document.
7. Completely unrelated HTML document.
8. Nested HTML elements.
9. Duplicate book entries.
10. HTML containing encoded characters.

### Metadata Enrichment

1. Successful Google Books lookup.
2. Successful Open Library lookup.
3. No matching book found.
4. Multiple possible matches.
5. Missing ISBN.
6. API timeout.
7. HTTP error response.
8. Missing author or publisher metadata.

External API tests should use mocked HTTP responses to ensure predictable test results.

### Export

1. Valid CSV output.
2. Valid JSON output.
3. Missing optional metadata.
4. Multiple authors.
5. Special characters in titles.
6. ISBN values preserved as strings.

## 10. Out of Scope

The following features are not part of the MVP:

- Direct integration with the Humble Bundle account.
- Automatic downloading of e-books.
- Database persistence.
- Web interface.
- Mobile application.
- User authentication.
- Manual metadata editing interface.
- Reading progress tracking.
- Book recommendations.
- Automatic categorization of books.

These features may be considered for future versions.

## 11. MVP Acceptance Criteria

The MVP is considered complete when:

1. The user can provide a local HTML file.
2. Complete HTML documents and HTML fragments are supported.
3. The application extracts recognizable book titles and publishers.
4. Missing or invalid information is reported without crashing the application.
5. Metadata enrichment runs automatically.
6. Missing metadata does not prevent catalog generation.
7. Uncertain metadata matches are distinguishable from verified matches.
8. The user can choose CSV or JSON export.
9. Both output formats contain the agreed catalog fields.
10. Automated tests cover the main parsing, enrichment, and export scenarios.

---

**End of Specification**