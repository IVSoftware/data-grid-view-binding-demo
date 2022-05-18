using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace dgv_ac
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Wait for the main form to be created, then attach 
        /// your Binding List as the data source of the DGV
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            dataGridView1.DataSource = this.DataSource;
            initDGV();
        }
        private void initDGV()
        {
            dataGridView1.AllowUserToAddRows = false;

            // Now you can populate the DataGridView simply
            // by adding some records to the list.
            for (int i = 0; i < 5; i++)
            {
                DataSource.Add(new Record { Number = i, FileName = $"MyFile_{i}.txt" });
            }
            dataGridView1.Refresh();

            // Once the first record is added, the Columns information
            // is available and we can do column formatting.
            dataGridView1.Columns[nameof(Record.FileName)].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            var numberColumn = dataGridView1.Columns[nameof(Record.Number)];
            numberColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            numberColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var checkboxColumn = dataGridView1.Columns[nameof(Record.IsChecked)];
            checkboxColumn.HeaderText = string.Empty;
            checkboxColumn.Width = 60;
            checkboxColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            attachPanel();

            dataGridView1.CellClick += onCellClick;
            dataGridView1.CellContentClick += onCellContentClick;
            dataGridView1.CellPainting += onCellPainting;
        }

        private void attachPanel()
        {
            var checkBox = new CheckBox()
            {
                Width = 20,
                Height = 20,
            };
            dataGridView1.Controls.Add(headerPanel);
            headerPanel.ColumnCount = 3;
            headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            headerPanel.Controls.Add(checkBox, 1, 0);
            headerPanel.AutoSize = true;
            checkBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        }

        // https://docs.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-host-controls-in-windows-forms-datagridview-cells?view=netframeworkdesktop-4.8


        private void onCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if ((e.RowIndex == -1) && (e.ColumnIndex != -1))
            {
                switch (dataGridView1.Columns[e.ColumnIndex].Name)
                {
                    case nameof(Record.IsChecked):
                        var headerCellLocation = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, -1, true);
                        headerPanel.Location = headerCellLocation.Location;
                        headerPanel.BackColor = Color.Red;
                        headerPanel.Size = headerCellLocation.Size;
                        break;
                }
            }
        }

        // https://www.aspsnippets.com/Articles/Add-Check-all-CheckBox-in-Header-row-of-DataGridView-using-C-and-VBNet-in-Windows-Application.aspx
        TableLayoutPanel headerPanel = new TableLayoutPanel();

        /// <summary>
        /// Detect check box click and end the edit mode in this case.
        /// </summary>
        private void onCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex != -1)
            {
                var cell = dataGridView1[e.ColumnIndex, e.RowIndex];
                if(cell is DataGridViewCheckBoxCell checkbox)
                {
                    dataGridView1.EndEdit();
                }
            }
        }


        /// <summary>
        /// Detect header click and set the records accordingly.
        /// </summary>
        private void onCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex == -1)
            {
                switch (dataGridView1.Columns[e.ColumnIndex].Name)
                {
                    case nameof(Record.IsChecked):
                        if (DataSource.Any())   // Check to see if there are any records at all.
                        {
                            if(DataSource.Count(record=>record.IsChecked) == DataSource.Count)
                            {
                                // This block says thet're all checked or all unchecked.
                                if(DataSource.First().IsChecked) // then they all are
                                {
                                    setAll(false);
                                }
                                else
                                {
                                    setAll(true);
                                }
                            }
                            else setAll(true); // If they're mixed, make them all checked.
                        }
                        break;
                }
            }
            void setAll(bool value)
            {
                foreach (var record in DataSource)
                {
                    record.IsChecked = value;
                }
                Refresh();
            }
        }       

        public BindingList<Record> DataSource = new BindingList<Record>();
    }
        // This is the record class that will provide column 
        // information to the DataGridView automatically.
    public class Record
    {
        public int Number { get; set; }
        public bool IsChecked { get; set; }
        public string FileName { get; set; }
    }
}
