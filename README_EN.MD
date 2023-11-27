# FilterableHeaderCell for DataGridView
*Read this in other languages: [简体中文](README.md)*
## Description
`FilterableHeaderCell` is a custom `DataGridViewColumnHeaderCell` that provides an easy-to-use filtering interface for users. This DataGridView enhancement allows users to filter items by selecting from a list of values in the column header. The component is written in C# and designed to be used with Windows Forms applications.

## Features
- Dropdown filter list integrated into the column header.
- Support for "All" selection to clear filters.
- Customizable appearance for the dropdown button and menu items.
- Efficient filtering for large datasets.

## Getting Started

### Prerequisites
- .NET Framework 4.5.2 or later.

### Installing
- Clone the repository or download the source code.
- Include `FilterableHeaderCell.cs` in your Windows Forms project.
- Build the project to ensure everything is set up correctly.

### Usage
To use `FilterableHeaderCell` in your project:
1. Assign `FilterableHeaderCell` to the `HeaderCell` property of your desired DataGridView column.
2. Populate `allFilterValues` with the unique values you want to be available for filtering.
3. Handle the `ApplyFilter` method to define how the data should be filtered based on the selected filters.

```csharp
var filterableHeaderCell = new FilterableHeaderCell();
myDataGridView.Columns["MyColumn"].HeaderCell = filterableHeaderCell;
