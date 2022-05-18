using System;
using System.ComponentModel;
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
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            dataGridView1.DataSource = this.DataSource;
            initDGV();
        }
        private void initDGV()
        {
            dataGridView1.AllowUserToAddRows = false;
            for (int i = 0; i < 5; i++)
            {
                DataSource.Add(new Record { Number = i, FileName = $"MyFile_{i}.txt" });
            }
            dataGridView1.Columns[nameof(Record.FileName)].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            var numberColumn = dataGridView1.Columns[nameof(Record.Number)];
            numberColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            numberColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var checkboxColumn = dataGridView1.Columns[nameof(Record.IsChecked)];
            checkboxColumn.HeaderText = string.Empty;
            checkboxColumn.Width = 40;

            dataGridView1.CellClick += onCellClick;
            dataGridView1.CellContentClick += onCellContentClick;
        }

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
                                // This block says thet're all chacked or all unchecked.
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
    public class Record
    {
        public int Number { get; set; }
        public bool IsChecked { get; set; }
        public string FileName { get; set; }
    }
}
