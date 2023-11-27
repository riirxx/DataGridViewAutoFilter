using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ClassLibrary;
using DevComponents.DotNetBar.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Drawing.Drawing2D;
public class FilterableHeaderCell : DataGridViewColumnHeaderCell
    {
        private Rectangle dropdownButtonRect = Rectangle.Empty;
        private bool dropdownOpen = false;
        private ContextMenuStrip filterMenu = new ContextMenuStrip();
        private HashSet<string> selectedFilters = new HashSet<string>();
        private List<string> allFilterValues = new List<string>();

        public FilterableHeaderCell()
        {
            filterMenu.ItemClicked += FilterMenu_ItemClicked;
            filterMenu.Closing += FilterMenu_Closing;
        }

        


        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
    DataGridViewElementStates dataGridViewElementState, object value, object formattedValue,
    string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
    DataGridViewPaintParts paintParts)
        {
            // Draw the default header cell
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState, value,
                formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            // Define the rectangle for the dropdown button relative to the cellBounds.
            dropdownButtonRect = new Rectangle(
                cellBounds.Right - 15, // 15 pixels from the right edge of the cellBounds
                cellBounds.Bottom - cellBounds.Height + 20,    // 5 pixels from the top edge of the cellBounds
                10,                    // 10 pixels wide
                cellBounds.Height - 20 // The height is the cell height minus 10 pixels
            );

            // Draw a custom dropdown button

            /*// Draw the dropdown button
            ControlPaint.DrawComboButton(graphics, dropdownButtonRect, ButtonState.Normal);
            if (dropdownOpen)
            {
                ControlPaint.DrawComboButton(graphics, dropdownButtonRect, ButtonState.Pushed);
            }*/
            graphics.FillRectangle(Brushes.LightGray, dropdownButtonRect); // Change the color as needed
            graphics.DrawRectangle(Pens.Black, dropdownButtonRect); // Draw border

            // Create the path for the arrow
            var arrowX = dropdownButtonRect.Left + dropdownButtonRect.Width / 2;
            var arrowY = dropdownButtonRect.Top + dropdownButtonRect.Height / 2;
            var arrowPath = new GraphicsPath();
            arrowPath.AddLine(arrowX - 3, arrowY - 1, arrowX + 3, arrowY - 1);
            arrowPath.AddLine(arrowX + 3, arrowY - 1, arrowX, arrowY + 2);
            arrowPath.CloseFigure();

            // Draw the arrow
            graphics.FillPath(Brushes.Black, arrowPath); // Change the color as needed

            if (dropdownOpen)
            {
                // Modify the appearance for when the dropdown is open
                graphics.FillRectangle(Brushes.Gray, dropdownButtonRect); // Change as needed
                graphics.FillPath(Brushes.Black, arrowPath); // Change the color as needed
            }
        }

        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            // 转换坐标，使之相对于列头单元格
            Point cellRelativeCoords = new Point(e.X + this.DataGridView.GetCellDisplayRectangle(e.ColumnIndex, -1, true).Left, e.Y);

            // 创建一个比可视按钮大的点击检测区域
            Rectangle largerClickArea = new Rectangle(
                dropdownButtonRect.Left - dropdownButtonRect.Width, // 扩大点击区域
                dropdownButtonRect.Top - dropdownButtonRect.Height,  // 扩大点击区域
                dropdownButtonRect.Width *2, // 扩大点击区域
                dropdownButtonRect.Height *2  // 扩大点击区域
            );

            if (largerClickArea.Contains(cellRelativeCoords))
            {
                
                this.DataGridView.InvalidateCell(this);
                // 变更下拉按钮的状态

                // 显示筛选菜单
                ShowFilterMenu();
            }



        }

        private void FilterMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem is ToolStripSeparator)
                return;
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)e.ClickedItem;
            string filterValue = clickedItem.Text;

            // Handle the "All" option
            if (filterValue == "All")
            {
                // If "All" is clicked, clear all filters and uncheck all other items
                selectedFilters.Clear();
                dropdownOpen = false;
                foreach (ToolStripMenuItem item in filterMenu.Items.OfType<ToolStripMenuItem>())
                {
                    item.Checked = false;
                }
                clickedItem.Checked = true; // Check the "All" item
            }
            else
            {
                // This is a regular filter item
                clickedItem.Checked = !clickedItem.Checked; // Toggle the checked state

                // Update the selected filters based on the item's checked state
                if (clickedItem.Checked)
                {
                    dropdownOpen = true;
                    selectedFilters.Add(filterValue);
                    // Uncheck the "All" item if any other item is checked
                    ToolStripMenuItem  allItem = (ToolStripMenuItem)filterMenu.Items[0];
                    allItem.Checked = false;
                }
                else
                {
                    selectedFilters.Remove(filterValue);
                }
            }

            
            ApplyFilter();
            clickedItem.Invalidate();

        }

        private void FilterMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {

            // Prevent the menu from closing when an item is clicked.
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
            {
                e.Cancel = true;
                (sender as ToolStripDropDownMenu).Invalidate();
            }
        }

        private void ShowFilterMenu()
        {
            InitializeFilterMenu();
            filterMenu.Show(this.DataGridView, this.DataGridView.GetCellDisplayRectangle(OwningColumn.Index, -1, true).Location);
        }

        private void InitializeFilterMenu()
        {
            filterMenu.Items.Clear();
            allFilterValues = GetUniqueValuesForColumn(this.OwningColumn.DataPropertyName);
            ToolStripMenuItem allItem = new ToolStripMenuItem("All")
            {
                CheckOnClick = false,
                Checked = selectedFilters.Count == 0 
            };
            
            filterMenu.Items.Add(allItem);
            filterMenu.Items.Add(new ToolStripSeparator()); // Add a separator
            foreach (string value in allFilterValues)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(value)
                {
                    CheckOnClick = false,
                    Checked = selectedFilters.Contains(value)
                };
                filterMenu.Items.Add(item);
            }
        }

        

        private void ApplyFilter()
        {
            if (this.DataGridView?.DataSource is BindingSource bindingSource)
            {
                string filterString = CreateFilterString(selectedFilters);
                bindingSource.Filter = filterString;
            }
            else
            {
               // Debug.WriteLine("dfsdf");
            
            }
            
        }

        private string CreateFilterString(HashSet<string> filters)
        {
            if (filters.Count == 0) return string.Empty;

            var filterParts = filters.Select(f => string.Format("[{0}] = '{1}'", this.OwningColumn.DataPropertyName, f.Replace("'", "''")));
            return string.Join(" OR ", filterParts);
        }

        private List<string> GetUniqueValuesForColumn(string dataPropertyName)
        {
            var uniqueValues = new HashSet<string>();
            foreach (DataGridViewRow row in this.DataGridView.Rows)
            {
                if (!row.IsNewRow)
                {
                    var value = row.Cells[dataPropertyName].Value?.ToString() ?? string.Empty;
                    uniqueValues.Add(value);
                }
            }
            return uniqueValues.ToList();
        }
    }
