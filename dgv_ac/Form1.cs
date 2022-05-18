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
            dataGridView1.CellContentClick += onCellContentClick;
            dataGridView1.CellPainting += onCellPainting;
            Record.CheckBoxChanged += (sender, e) =>
            {
                if (DataSource.Any())   // Check to see if there are any records at all.
                {
                    var checkedCount = DataSource.Count(record => record.IsChecked);
                    if(checkedCount == DataSource.Count)
                    {
                        // All checked
                        _checkBox.CheckState = CheckState.Checked;
                    }
                    else if (checkedCount == 0)
                    {
                        _checkBox.CheckState = CheckState.Unchecked;
                    }
                    else _checkBox.CheckState = CheckState.Indeterminate;
                }
            };
        }

        private void attachPanel()
        {
           _checkBox = new CheckBox()
            {
                Width = 20,
                Height = 20,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
                AutoCheck = false,
            };
            _checkBox.Click += onCheckBoxClick;
            dataGridView1.Controls.Add(_headerPanel);
            _headerPanel.ColumnCount = 3;
            _headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            _headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            _headerPanel.Controls.Add(_checkBox, 1, 0);
            _headerPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            _headerPanel.AutoSize = true;
        }

        private void onCheckBoxClick(object sender, EventArgs e)
        {
            if (DataSource.Any())   // Check to see if there are any records at all.
            {
                if (DataSource.Count(record => record.IsChecked) == DataSource.Count)
                {
                    // This block says thet're all checked or all unchecked.
                    if (DataSource.First().IsChecked) // then they all are
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
                        _headerPanel.Location = headerCellLocation.Location;
                        _headerPanel.Size = headerCellLocation.Size;
                        break;
                }
            }
        }

        // https://www.aspsnippets.com/Articles/Add-Check-all-CheckBox-in-Header-row-of-DataGridView-using-C-and-VBNet-in-Windows-Application.aspx
        TableLayoutPanel _headerPanel = new TableLayoutPanel();
        CheckBox _checkBox;

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
        private void setAll(bool value)
        {
            foreach (var record in DataSource)
            {
                record.IsChecked = value;
            }
            BeginInvoke((MethodInvoker)delegate { _checkBox.Checked = value; });
            Refresh();
        }       

        public BindingList<Record> DataSource = new BindingList<Record>();
    }
        // This is the record class that will provide column 
        // information to the DataGridView automatically.
    public class Record
    {
        public int Number { get; set; }
        private bool _isChecked;
        public bool IsChecked 
        { 
            get => _isChecked; 
            set
            {
                if(_isChecked != value)
                {
                    _isChecked = value;
                    CheckBoxChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public static event EventHandler CheckBoxChanged;
        public string FileName { get; set; }
    }
}
