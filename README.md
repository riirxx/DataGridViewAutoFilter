# DataGridView中的FilterableHeaderCell
*阅读其他语言版本： [English](README_EN.md)*
## 描述
`FilterableHeaderCell` 是一个自定义的 `DataGridViewColumnHeaderCell`，它为用户提供了一个易于使用的筛选界面。这个DataGridView增强功能允许用户通过从列头部的值列表中选择来筛选项目。该组件使用C#编写，旨在与Windows Forms应用程序一起使用。

## 特点
- 下拉筛选列表集成到列头部。
- 支持选择“全部”来清除筛选。
- 可自定义下拉按钮和菜单项的外观。
- 高效筛选大型数据集。

## 开始使用

### 先决条件
- .NET Framework 4.5.2或更高版本。

### 安装
- 克隆仓库或下载源代码。
- 将 `FilterableHeaderCell.cs` 包含到您的Windows Forms项目中。
- 构建项目以确保一切设置正确。

### 使用方法
在您的项目中使用 `FilterableHeaderCell`：
1. 将 `FilterableHeaderCell` 赋值给您所需的DataGridView列的 `HeaderCell` 属性。
2. 用您希望可供筛选的唯一值填充 `allFilterValues`。
3. 处理 `ApplyFilter` 方法，以定义如何根据选定的筛选器筛选数据。

```csharp
var filterableHeaderCell = new FilterableHeaderCell();
myDataGridView.Columns["MyColumn"].HeaderCell = filterableHeaderCell;
