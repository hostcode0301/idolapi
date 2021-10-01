# Idol Project

Basic ASP.NET Core API architecture project

## Installation

Use dotnet cli to install all packages in Idol Project.

```bash
dotnet restore idolapi.csproj
```

## Usage

Demo main functions

## Function detail

### Pagination:

-   (Anonymous) GET /api/idol/page?pageNumber=1&pageSize=3&searchName=demo

### Excel exporting:

-   (Anonymous) GET /api/idol/excel

### Excel importing:

-   (Anonymous) POST /api/idol/excel

### Notification app:

-   (Anonymous) /hubs/notification
