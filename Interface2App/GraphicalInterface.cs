﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Timers;
using System.Xml;
using VisualStimuli;
using System.Xml.Serialization;

namespace Interface2App
{
	public partial class Form1 : Form
	{
        public static string path;
        public static string default_save_file;
		public BindingSource dataview;
		private bool FlickerRunning=false;
		private static Color convertColor(System.Windows.Media.Color c)
		{
			return Color.FromArgb(c.A,c.R,c.G,c.B);
		}
        public Form1()
		{
			InitializeComponent();
            path = Application.StartupPath;
            path = path.Substring(0, path.LastIndexOf('\\'));
            path = path.Substring(0, path.LastIndexOf('\\'));
			default_save_file = path + "\\Flickers.xml";
        }
		private List<Flicker> FlickerList = new List<Flicker>();
		private List<Flicker> CopyList = new List<Flicker>();
        /// <summary>
        /// Loading the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
		{
            flickerBindingSource.DataSource = FlickerList;
			//screenViewer1.DataBindings.Add("DataSource",FlickerList , "", true, DataSourceUpdateMode.OnPropertyChanged);
			screenViewer1.form = this;
			dataview = flickerBindingSource;
			// Get the path of the selected file
			if (File.Exists(default_save_file))
			{
                loadFile(default_save_file);
            }
			
        }
		delegate void SetTextCallback(Label label, string text);
		/// <summary>
		/// used for updating the label on screen. Mostly used for error and test
		/// </summary>
		/// <param name="l"></param>
		/// <param name="text"></param>
		public void SetText(Label l, string text)
		{
			if (l.InvokeRequired)
			{
				SetTextCallback d = new SetTextCallback(SetText);
				Invoke(d, new object[] { l, text });
			}
			else
			{
				l.Text = text;
			}
		}
		
		/// <summary>
		/// Load an XML file of flickers and actualize the DataGridView and ScreenViewer
		/// </summary>
		/// <param name="filePath"></param>
		
		private void loadFile(string filePath)
		{
            XmlSerializer serializer = new XmlSerializer(typeof(List<Flicker>));
            using (StreamReader s = new StreamReader(filePath))
            {
                FlickerList = (List<Flicker>)serializer.Deserialize(s);
                s.Close();
                flickerBindingSource.DataSource = FlickerList;
                FlickerDataGridView.Update();
                //SetText(labelTest, FlickerList.Count.ToString());
                for (int i=0; i < FlickerList.Count; i++)
				{
                    FlickerDataGridView.Rows[i].Cells["color"].Style.BackColor = convertColor(FlickerList[i].color1);
					if (FlickerList[i].IsImageFlicker)
					{
						FlickerDataGridView.Rows[i].Cells["color"].Value = FlickerList[i].image;

                    }
                }
                screenViewer1.InvalidateRectangle();
            }
        }
		/// <summary>
		/// Saving all informations which were written in the interface to an xml file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bt_save(object sender, EventArgs e)
		{
			try
			{
                string file = default_save_file;
                XmlSerializer serializer = new XmlSerializer(typeof(List<Flicker>));
				using (StreamWriter s = new StreamWriter(file))
				{
					serializer.Serialize(s, FlickerList);
					s.Close();
				}
                SetText(labelTest, string.Format("File saved Succesfully at: {0}", file));
			}
			catch
			{
				SetText(labelTest, "An Error Occured While Trying To Save");
			}
            
			return;
		}
		/// <summary>
		/// save as an XML file the current flickers.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void bt_save_as(object sender, EventArgs e)
        {

            try
            {
                // Create an instance of the SaveFileDialog class
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                // Set the filter to only show files with the .txt extension
                saveFileDialog.Filter = "Xml Files (*.xml)|*.xml|All files (*.*)|*.*";
				saveFileDialog.DefaultExt = "xml";

                // Set the default directory to the App directory
                saveFileDialog.InitialDirectory = path;

				string file;
                // Show the dialog and check if the user clicked the OK button
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the path of the selected file
                    file = (string)saveFileDialog.FileName;
				}
				else
				{
					file = path + "\\Flickers.xml";
				}
                XmlSerializer serializer = new XmlSerializer(typeof(List<Flicker>));
                using (StreamWriter s = new StreamWriter(file))
                {
                    serializer.Serialize(s, FlickerList);
                    s.Close();
                }
                SetText(labelTest, string.Format("File saved Succesfully at: {0}", file));
				default_save_file = file;
            }
            catch
            {
                SetText(labelTest, "An Error Occured While Trying To Save");
            }

            return;

        }
		

		// Closing Application in anyway
		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			System.Windows.Forms.Application.Exit();
		}
		/// <summary>
		/// Indicating instructions 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_help_Click(object sender, EventArgs e)
		{
			System.Windows.Forms.MessageBox.Show("I.\n"
				+ "  X (Horizontal), Y (Vertical) correspond to the position of top-left point of a Flicker (in pixel).\n"
				+ "II.\n"
				+ " Width, Height is the size of a Flicker (in pixel).\n"
				+ "III.\n"
				+ " Frequency in Hz and Phase in degrees.\n"
				+ "IV.\n"
				+ " You can choose in Type\n"
				+ " Random \n"
				+ " Sinous \n"
				+ " Square \n"
				+ " Root Square\n"
				+ " Maximum length sequence\n"
				+ "V.\n"
				+ " You can click on New to create a new Flicker \n"
				+ "VI.\n"
				+ " Finally, click on RUN to run the Flicker program or TEST for a 10 seconds test.\n"
				+ " Click on a flicker and press 'Echap' to close all flickers.\n"
				+ "THANK FOR READING !!!");
		}
		/// <summary>
		/// Run a 10 seconds test and show flickers on screen
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_pre_Click(object sender, EventArgs e)
		{
            bt_save(sender, e);
            if (!FlickerRunning)
			{
				flickerBindingSource.EndEdit();
                CPlay oPlay = new CPlay();
                var t = new Thread(oPlay.Animate_Flicker);
                t.Start();
				FlickerRunning= true;
                System.Timers.Timer timer = new System.Timers.Timer(10000);
                timer.AutoReset = false;
                timer.Elapsed += OnElapsed;
				timer.Start();
                void OnElapsed(object sender1, EventArgs e1)
                {
                    t.Abort();
					FlickerRunning= false;
                }
            }
            //Application.Exit();
        }
		/// <summary>
		/// Run a thread to show flickers on screen
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bt_run_Click(object sender, EventArgs e)
		{
			bt_save(sender, e);
			if (this.ValidateChildren(ValidationConstraints.ImmediateChildren | ValidationConstraints.Enabled) && !FlickerRunning)
			{

				CPlay oPlay = new CPlay();
				new Thread(oPlay.Animate_Flicker).Start();
				FlickerRunning= true;
				//Application.Exit();
			}
		}


		// errorProvider 
		//----------------------------------------------- Input validation ----------------------------------------------//

		public int resX = Screen.PrimaryScreen.Bounds.Width;
		public int resY = Screen.PrimaryScreen.Bounds.Height;

		/// <summary>
		/// add a new flicker
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_new_Click(object sender, EventArgs e)
		{
			flickerBindingSource.AddNew();
			FlickerDataGridView.Rows[FlickerList.Count-1].Cells["color"].Style.BackColor = convertColor(FlickerList[FlickerList.Count - 1].color1);
        }
		/// <summary>
		/// delete selected flickers in the datagridview
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_delete_Click(object sender, EventArgs e)
		{
			foreach (DataGridViewRow item in this.FlickerDataGridView.SelectedRows)
			{
				FlickerDataGridView.Rows.RemoveAt(item.Index);
			}
		}
		/// <summary>
		/// import a list of flicker from a XML file, remove the current flickers!
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_imp(object sender, EventArgs e)
		{
            // Create an instance of the OpenFileDialog class
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set the filter to only show files with the .txt extension
            openFileDialog.Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*";

			// Set the default directory to the user's My Documents folder
			openFileDialog.InitialDirectory = path;

            // Show the dialog and check if the user clicked the OK button
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the path of the selected file
                string filePath = openFileDialog.FileName;
				loadFile(filePath);
            }
        }
		/// <summary>
		/// update the screenViewer since it doesn't use dataBinding.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void onDataChanged(object sender, EventArgs e)
		{
			screenViewer1.DataSource= FlickerList;
			screenViewer1.InvalidateRectangle(FlickerDataGridView.CurrentRow.Index); //update only at the correct row to reduce calculation
		}
		/// <summary>
		/// function used for custom interaction with datagridview cells.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void FlickerDataGridCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
			try
			{
				if (e.RowIndex >= 0 && e.ColumnIndex == FlickerDataGridView.Columns["color"].Index)
				{
                    //SetText(labelTest, "test");
                    if (FlickerList[e.RowIndex].IsImageFlicker)
					{
						OpenFileDialog imageDialog= new OpenFileDialog();
						imageDialog.Filter = "Bmp image File (*.bmp)|*.bmp|All files (*.*)|*.*";
						imageDialog.InitialDirectory = path;
						if(imageDialog.ShowDialog() == DialogResult.OK)
						{
							FlickerList[e.RowIndex].image = imageDialog.FileName;
							FlickerDataGridView.Rows[e.RowIndex].Cells["color"].Style.BackColor = Color.White;
                            FlickerDataGridView.Rows[e.RowIndex].Cells["color"].Value = imageDialog.FileName;
                        }
					}
					else
					{
                        ColorDialog colorPickerDialog = new ColorDialog();
                        if (colorPickerDialog.ShowDialog() == DialogResult.OK)
                        {
                            var c1 = colorPickerDialog.Color;
                            FlickerList[e.RowIndex].color1 = System.Windows.Media.Color.FromArgb(c1.A, c1.R, c1.G, c1.B);
                            FlickerDataGridView.Rows[e.RowIndex].Cells["color"].Style.BackColor = c1;
                        }
                    }
				}
                onDataChanged(sender, e);
            }
			catch (Exception ex)
			{
				SetText(labelTest,ex.Message+ex.StackTrace);
			}
            
        }
		/// <summary>
		/// catch keys pressed for shortcut implementation
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            System.Windows.Forms.KeyEventArgs e = new System.Windows.Forms.KeyEventArgs(keyData);
            //SetText(labelTest, "Key pressed down");
            // Check if the pressed key combination matches a defined shortcut
            if (e.Control && e.KeyCode == Keys.N)
            {
                // The pressed key combination matches the Ctrl+N shortcut
                // Simulate a click on the "new" button
                btn_new.PerformClick();
				return true;
            }
            // Check if the pressed key combination matches a defined shortcut
            if (e.Control && e.KeyCode == Keys.S)
            {
				// The pressed key combination matches the Ctrl+S shortcut
				// Simulate a click on the save button
				if (e.Shift)
				{
					buttonSaveAs.PerformClick();
				}
				else
				{
                    btn_save.PerformClick();
                }
                
				return true;
            }
			if (e.Control && e.KeyCode == Keys.C) { 
				CopyList.Clear();
				if(this.FlickerDataGridView.SelectedRows.Count > 0 && flickerBindingSource.Count>0) {
                    foreach (DataGridViewRow item in this.FlickerDataGridView.SelectedRows)
                    {
                        CopyList.Add(FlickerList[item.Index]);
                    }
					Clipboard.SetDataObject(CopyList); //add to clipboard but not used inside this code, useful if you want to send it to other people
                }
				
			}
            if (e.Control && e.KeyCode == Keys.V)
            {
                if(CopyList.Count > 0)
				{
					//FlickerList.AddRange(CopyList);
					foreach(Flicker f in CopyList) { flickerBindingSource.Add(f); }
				}
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
		//when binding are reset (after a rectangle on screen was modified), colors and path are reset in the table, remodifying
        private void flickerBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
			var dgv = FlickerDataGridView;
            for (int i = 0; FlickerList.Count > i; i++)
            {
				if (!FlickerList[i].IsImageFlicker)
				{
                    dgv.Rows[i].Cells["color"].Style.BackColor = convertColor(FlickerList[i].color1);
				}
				else
				{
                    dgv.Rows[i].Cells["color"].Value = FlickerList[i].image;
                }
                
            }
        }
    }
}
