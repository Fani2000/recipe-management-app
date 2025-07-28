# Project Documentation: Recipe Management App (RMA)

---

## Overview

The **Recipe Management App (RMA)** is a full-stack application that allows users to create, view, search, and manage cooking recipes. The system supports features like tagging, image preview, step-by-step cooking instructions, caching, validation, and benchmarked performance testing.

This documentation covers the architecture, key components, and implementation details from frontend to backend.

---

## System Architecture

```
+---------------------------+             +-----------------------------+              +-----------------------------+
|        Frontend          |             |       API Gateway / BFF     |              |         Backend             |
| (Vue 3 + Vuetify + TS)   | <---------> |  ASP.NET Core Controller     | <----------> |  ASP.NET Core Service Layer |
| - Responsive UI/UX       |             |  - Validation & Routing      |              |  - Business Logic           |
| - Tabs, Forms, Layout    |             |  - ModelState Validations    |              |  - EF Core (DbContext)      |
| - Floating buttons       |             |                             |              |  - Redis Caching            |
+---------------------------+             +-----------------------------+              +-----------------------------+
                                                                                                 |
                                                                                                 v
                                                                                      +----------------------------+
                                                                                      |      Database (SQL)       |
                                                                                      | - Recipes, Tags, etc.     |
                                                                                      +----------------------------+
```

---

## Technologies Used

### Frontend

* Vue 3 Composition API
* Vuetify 3 (Material UI)
* TypeScript
* Responsive layout with grid system
* Axios for API calls

### Backend

* ASP.NET Core Web API
* Entity Framework Core
* Redis Caching
* xUnit for Unit/Integration Tests
* FluentValidation (optional for extensibility)
* BenchmarkDotNet (for load testing)

### Testing & Performance

* Unit Tests with xUnit
* Integration Tests using `DistributedApplicationTestingBuilder`
* Benchmarks for GET `/api/Recipes` (500x sequential calls)

---

## Key Features

### 🎨 Frontend UI/UX

* Recipe form with full-page, tabbed layout
* Validation-aware forms using Vuetify components
* Floating action buttons
* Expandable panels for ingredients/steps
* Gradient overlays for image headers

### 🧠 Backend Logic

* Strong validation using `[Required]`, `[Range]`, and `[StringLength]`
* Centralized `RecipeService` for handling business logic
* Entity relations: Recipe -> Ingredients, Steps, Tags
* Caching with Redis for performance optimization

### 🔎 Search & Filtering

* Query `/api/Recipes?search=...&tag=...&maxCookingTime=...`
* Searches across title, description, and tag names (case-insensitive)
* Supports sorting by `title` or `cookingTime`

### 📊 Performance

**Benchmark Summary:**

```
| Method             | Mean      | Error     | StdDev    |
|-------------------|-----------|-----------|-----------|
| GetAllRecipes_500 | 138.42 ms | ±3.05 ms  | ±3.97 ms  |
```

---

## Validation Rules

**Recipe**:

* `Title`: Required, max 150 chars
  G* `Description`: Required, max 1000 chars
* `Image`: Required, must be a valid URL
* `CookingTimeMinutes`: Required, 1 to 1440 minutes

Additional validation can be added for `Ingredients`, `Steps`, and `Tags`.

---

## API Endpoints

### GET /api/Recipes

Returns all recipes with optional filters:

* `search`, `tag`, `maxCookingTime`, `sortBy`, `ascending`

### GET /api/Recipes/{id}

Returns a single recipe by ID

### POST /api/Recipes

Creates a new recipe (with validation)

### PUT /api/Recipes/{id}

Updates a recipe (with validation)

### DELETE /api/Recipes/{id}

Deletes a recipe by IDggV

---

## Example Query

```http
GET /api/Recipes?search=pasta&tag=vegan&maxCookingTime=30&sortBy=title&ascending=true
```

---

## Testing Coverage

* ✅ Success flow: create + fetch recipe
* ✅ Validation failure (missing title, invalid time)
* ✅ Benchmark testing (500x GETs)

Console output confirms:

* Test case name
* Status
* Average request latency

---

## Future Enhancements

* ✅ Add FluentValidation for more complex model rules
* ✅ Role-based access/authentication
* ✅ Pagination & infinite scroll on frontend
* ✅ Advanced filters (e.g., exclude ingredients)

---

## Summary

This app provides a robust, full-stack implementation for managing recipes with a modern frontend and efficient backend architecture. It is testable, validated, and optimized for performance.
