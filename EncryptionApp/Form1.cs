using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace EncryptionApp
{
	public partial class Form1 : Form
	{

		private bool resultIsSaved = false;
		private string encryptionResult = "null";

		private delegate void SafeCallDelegate(string text);

		public enum EncryptionType
		{
			Cesar,
			TextBased,
			Substitution,
			Polybe,
			ToBinary,
			Unknown
		}

		public Form1()
		{
			InitializeComponent();
			ShowEncryptionTypeof(GetCurrentEncryptionType());
		}

		private void resetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			cesarRadioButton.Checked = true;

			foreach (Control control in Controls)
			{
				switch (control)
				{
					case TextBox tb:
						tb.Text = "";
						break;

					case GroupBox gb:
						foreach (Control ct in gb.Controls)
						{
							if (ct.GetType().Equals(typeof(TextBox)))
							{
								ct.Text = "";
							}
						}
						break;

					default:
						break;
				}
			}
		}

		private void addAtxtFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool openFile = true;
			if (!inputText.Text.Equals("") && MessageBox.Show("Do you want to override current input text?", "Warning",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.No))
			{
				openFile = false;
			}
			if (openFile)
			{
				openFileDialog1.Filter = "Text Files|*.txt";
				openFileDialog1.RestoreDirectory = true;
				try
				{
					if (openFileDialog1.ShowDialog().Equals(DialogResult.OK))
					{
						StreamReader reader = new StreamReader(openFileDialog1.OpenFile());
						inputText.Text = reader.ReadLine();
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Unexpected Error occured typeof Excetption :: " + ex.GetType() + " Message :: " + ex.Message,
						ex.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void saveResultToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!resultBox.Text.Equals(""))
			{
				saveFileDialog1.Filter = "Text Files|*.txt";
				saveFileDialog1.FileName = "EncryptionResult";
				saveFileDialog1.RestoreDirectory = true;

				try
				{
					if (saveFileDialog1.ShowDialog().Equals(DialogResult.OK))
					{
						Cursor = Cursors.WaitCursor;
						StreamWriter writer = new StreamWriter(saveFileDialog1.OpenFile());
						//for (int i = 0; i < resultBox.Text.Length; i++)
						//{
						//    writer.Write(resultBox.Text[i]);
						//    if (i % 145 == 0 && i > 1)
						//    {
						//        writer.WriteLine();
						//    }
						//}
						writer.Write(resultBox.Text);
						writer.Dispose();
						writer.Close();
						resultIsSaved = true;
						Cursor = Cursors.Default;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Unexpected Error occured typeof Excetption :: " + ex.GetType() + " Message :: " + ex.Message,
						ex.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				MessageBox.Show("Please Encrypt Something Before Saving it !", "Error",
					MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
		}

		private void resultBox_TextChanged(object sender, EventArgs e)
		{
			resultIsSaved = false;
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Form2 form2 = new Form2();
			form2.Show();
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);
			if (e.CloseReason.Equals(CloseReason.WindowsShutDown)) return;

			if (!resultBox.Text.Equals("") && !resultIsSaved)
			{
				switch (MessageBox.Show("Are you sure you want to close the Application without saving?", "Warning", MessageBoxButtons.YesNo
					, MessageBoxIcon.Warning))
				{
					case DialogResult.No:
						e.Cancel = true;
						break;

					default:
						break;
				}
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			//Thread encryptionThread = new Thread(() => ProcessEncryptionTypeof(inputText.Text, GetCurrentEncryptionType()));
			//encryptionThread.Start();
			ProcessEncryptionTypeof(inputText.Text, GetCurrentEncryptionType());
			resultBox.Text = encryptionResult;
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://www.fileformat.info/info/charset/UTF-8/list.htm");
		}

		private void polybeDefaultTableCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (polybeDefaultTableCheckbox.Checked)
			{
				SetColumns(polybeTable, 6);
				char[,] tableOfChars =
				{
				   {'a','b','c','d','e','f'},
				   {'g','h','i','j','k','l'},
				   {'m','n','o','p','q','r'},
				   {'s','t','u','v','w','x'},
				   {'y','z', '\0','\0','\0','\0'}
				};
				for (int i = 0; i < tableOfChars.GetLength(0); i++)
				{
					for (int j = 0; j < tableOfChars.GetLength(1); j++)
					{
						polybeTable[j, i].Value = tableOfChars[i, j];
					}
				}
			}
			else
			{
				SetColumns(polybeTable, 6);
			}
		}

		private void columnsCountPolybeEncryption_ValueChanged(object sender, EventArgs e)
		{
			SetColumns(polybeTable, (int)columnsCountPolybeEncryption.Value);
		}

		private void cesarOffsetGenerateRandomOffsetButton_Click(object sender, EventArgs e)
		{
			Random randomNumber = new Random();
			cesarEncryptionOffset.Value = randomNumber.Next(Convert.ToInt32(-26),
				Convert.ToInt32(26));
		}

		private void substitutionGenerateRandomSubstitutionsButton_Click(object sender, EventArgs e)
		{
			Random randomNumber = new Random();
			List<char> alreadyGeneratedChars = new List<char>();
			List<TextBox> textBoxes = new List<TextBox>();
			int maxUTF8Value = 126;
			int minUTF8Value = 33;

			#region Hardcoded textBoxes

			textBoxes.Add(substitutionChar0); textBoxes.Add(substitutionChar1); textBoxes.Add(substitutionChar2);
			textBoxes.Add(substitutionChar3); textBoxes.Add(substitutionChar4); textBoxes.Add(substitutionChar5);
			textBoxes.Add(substitutionChar6); textBoxes.Add(substitutionChar7); textBoxes.Add(substitutionChar8);
			textBoxes.Add(substitutionChar9); textBoxes.Add(substitutionChar10); textBoxes.Add(substitutionChar11);
			textBoxes.Add(substitutionChar12); textBoxes.Add(substitutionChar13); textBoxes.Add(substitutionChar13);
			textBoxes.Add(substitutionChar14); textBoxes.Add(substitutionChar15); textBoxes.Add(substitutionChar16);
			textBoxes.Add(substitutionChar17); textBoxes.Add(substitutionChar18); textBoxes.Add(substitutionChar19);
			textBoxes.Add(substitutionChar20); textBoxes.Add(substitutionChar21); textBoxes.Add(substitutionChar22);
			textBoxes.Add(substitutionChar23); textBoxes.Add(substitutionChar24); textBoxes.Add(substitutionChar25);

			#endregion Hardcoded textBoxes

			foreach (TextBox textBox in textBoxes)
			{
				char character;
				do
				{
					character = char.ToLower((char)randomNumber.Next(minUTF8Value, maxUTF8Value));
				} while (alreadyGeneratedChars.Contains(character));
				alreadyGeneratedChars.Add(character);
				textBox.Text = Convert.ToString(character);
			}
		}

		private void textBasedGenerateRandomKeyButton_Click(object sender, EventArgs e)
		{
			int minUTF8Value = 97;
			int maxUTF8Value = 122;
			Random randomNumber = new Random();
			int randomKeySize = Convert.ToInt32(textBasedRandomKeySize.Value);
			string randomKey = null;

			for (int i = 0; i < randomKeySize; i++)
			{
				randomKey = randomKey + Convert.ToChar(randomNumber.Next(minUTF8Value, maxUTF8Value + 1));
			}

			textBasedEncryptionKey.Text = randomKey;
		}

		private void cesarRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			ShowEncryptionTypeof(EncryptionType.Cesar);
		}

		private void textBasedRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			ShowEncryptionTypeof(EncryptionType.TextBased);
		}

		private void substitutionRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			ShowEncryptionTypeof(EncryptionType.Substitution);
		}

		private void polybeRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			ShowEncryptionTypeof(EncryptionType.Polybe);
		}

		private void toBinaryRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			ShowEncryptionTypeof(EncryptionType.ToBinary);
		}

		private void ShowEncryptionTypeof(EncryptionType unhidedElementsRelatedToType)
		{
			foreach (EncryptionType type in Enum.GetValues(typeof(EncryptionType)))
			{
				switch (type)
				{
					case EncryptionType.Cesar:
						cesarSettersGroupBox.Visible = false;
						break;

					case EncryptionType.TextBased:
						textBasedEncryptionSettersGroupBox.Visible = false;
						break;

					case EncryptionType.Substitution:
						substitutionEncryptionSettersGroupBox.Visible = false;
						break;

					case EncryptionType.Polybe:
						polybeEncryptionSettersGroupBox.Visible = false;
						polybeDefaultTableCheckbox.Checked = false;
						break;

					case EncryptionType.ToBinary:
						toBinaryEncryptionSettersGroupBox.Visible = false;
						break;

					default:
						break;
				}
			}
			switch (unhidedElementsRelatedToType)
			{
				case EncryptionType.Cesar:
					flowLayoutPanel1.Controls.Add(cesarSettersGroupBox);
					cesarSettersGroupBox.Visible = true;
					break;

				case EncryptionType.TextBased:
					flowLayoutPanel1.Controls.Add(textBasedEncryptionSettersGroupBox);
					textBasedEncryptionSettersGroupBox.Visible = true;
					break;

				case EncryptionType.Substitution:
					flowLayoutPanel1.Controls.Add(substitutionEncryptionSettersGroupBox);
					SetColumns(polybeTable, (int)columnsCountPolybeEncryption.Value);
					substitutionEncryptionSettersGroupBox.Visible = true;
					break;

				case EncryptionType.Polybe:
					flowLayoutPanel1.Controls.Add(polybeEncryptionSettersGroupBox);
					SetColumns(polybeTable, Convert.ToInt32(columnsCountPolybeEncryption.Value));
					polybeEncryptionSettersGroupBox.Visible = true;
					break;

				case EncryptionType.ToBinary:
					flowLayoutPanel1.Controls.Add(toBinaryEncryptionSettersGroupBox);
					toBinaryEncryptionSettersGroupBox.Visible = true;
					break;

				default:
					break;
			}
		}

		public string ProcessEncryptionTypeof(string input, EncryptionType encryptionType)
		{
			try
			{
				lock (encryptionResult)
				{
					Stopwatch watch = new Stopwatch();
					watch.Start();
					Cursor = Cursors.WaitCursor;
					switch (encryptionType)
					{
						case EncryptionType.Cesar:
							try
							{
								//resultBox.Text = Encryption.CesarEncryption(inputText.Text,
								//    Convert.ToInt32(cesarEncryptionOffset.Value));

								encryptionResult = Encryption.CesarEncryption(input,
										  Convert.ToInt32(cesarEncryptionOffset.Value));
							}
							catch (Exception e)
							{
								MessageBox.Show(e.GetType().ToString() + " EXCEPTION: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
							break;

						case EncryptionType.TextBased:
							try
							{
								//resultBox.Text = Encryption.TextBasedEncryption(inputText.Text,
								//    textBasedEncryptionKey.Text);

								encryptionResult = Encryption.TextBasedEncryption(input,
									textBasedEncryptionKey.Text);
							}
							catch (Exception e)
							{
								if (e.GetType().Equals(typeof(IndexOutOfRangeException)))
								{
									MessageBox.Show("Please fill the Key text box !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
								}
								else
								{
									MessageBox.Show(e.GetType().ToString() + " EXCEPTION: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
								}
							}
							break;

						case EncryptionType.Substitution:
							try
							{
								List<char> chars = new List<char>();

								#region Adding Chars To List

								chars.Add(substitutionChar0.Text[0]);
								chars.Add(substitutionChar1.Text[0]);
								chars.Add(substitutionChar2.Text[0]);
								chars.Add(substitutionChar3.Text[0]);
								chars.Add(substitutionChar4.Text[0]);
								chars.Add(substitutionChar5.Text[0]);
								chars.Add(substitutionChar6.Text[0]);
								chars.Add(substitutionChar7.Text[0]);
								chars.Add(substitutionChar8.Text[0]);
								chars.Add(substitutionChar9.Text[0]);
								chars.Add(substitutionChar10.Text[0]);
								chars.Add(substitutionChar11.Text[0]);
								chars.Add(substitutionChar12.Text[0]);
								chars.Add(substitutionChar13.Text[0]);
								chars.Add(substitutionChar14.Text[0]);
								chars.Add(substitutionChar15.Text[0]);
								chars.Add(substitutionChar16.Text[0]);
								chars.Add(substitutionChar17.Text[0]);
								chars.Add(substitutionChar18.Text[0]);
								chars.Add(substitutionChar19.Text[0]);
								chars.Add(substitutionChar20.Text[0]);
								chars.Add(substitutionChar21.Text[0]);
								chars.Add(substitutionChar22.Text[0]);
								chars.Add(substitutionChar23.Text[0]);
								chars.Add(substitutionChar24.Text[0]);
								chars.Add(substitutionChar25.Text[0]);

								#endregion Adding Chars To List

								//resultBox.Text = Encryption.SubstitutionEncryption(inputText.Text, chars);

								encryptionResult = Encryption.SubstitutionEncryption(input, chars);
							}
							catch (Exception e)
							{
								if (e.Equals(typeof(IndexOutOfRangeException)))
								{
									MessageBox.Show("Fill all characters substitutions please!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
								}
								else
								{
									MessageBox.Show(e.GetType().ToString() + " EXCEPTION: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
								}
							}
							break;

						case EncryptionType.Polybe:
							try
							{
								char[,] table = new char[polybeTable.RowCount, polybeTable.ColumnCount];
								for (int i = 0; i < table.GetLength(0); i++)
								{
									for (int j = 0; j < table.GetLength(1); j++)
									{
										if (polybeTable[j, i].Value != null)
											table[i, j] = Convert.ToChar(polybeTable[j, i].Value);
									}
								}
								if (lineColumnTypeRadioButtonPolybe.Checked)
								{
									encryptionResult = Encryption.PolybeEncryption(input, table, Encryption.PolybeType.LineColumn);
								}
								else if (columnLineTypeRadioButtonPolybe.Checked)
								{
									encryptionResult = Encryption.PolybeEncryption(input, table, Encryption.PolybeType.ColumnLine);
								}
							}
							catch (Exception e)
							{
								MessageBox.Show(e.GetType().ToString() + " EXCEPTION: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
							break;

						case EncryptionType.ToBinary:
							try
							{
								string separator = separatorToBinaryEncryption.Text;
								int fillBits = Convert.ToInt32(fillBitsToBinaryEncryption.Value);

								if (normalTypeRadioButtonToBinary.Checked)
								{
									encryptionResult = Encryption.ToBinaryEncryption(input, Encryption.ToBinaryType.Normal, fillBits, separator);
								}
								else if (cp1TypeRadioButtonToBinary.Checked)
								{
									encryptionResult = Encryption.ToBinaryEncryption(input, Encryption.ToBinaryType.CP1, fillBits, separator);
								}
								else if (cp2TypeRadioButtonToBinary.Checked)
								{
									encryptionResult = Encryption.ToBinaryEncryption(input, Encryption.ToBinaryType.CP2, fillBits, separator);
								}
							}
							catch (Exception e)
							{
								MessageBox.Show(e.GetType().ToString() + " EXCEPTION: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
							break;

						default:
							throw new Exception("Unexpected Exception" +
								"\nWhooops! Something went wrong::Unkown encryption type");
					}
					Cursor = Cursors.Default;
					watch.Stop();
					elapsedTimeLabel.Text = watch.Elapsed.TotalSeconds.ToString() + "s";

					return encryptionResult;
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.GetType().ToString() + " EXCEPTION: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return null;
			}
		}

		private EncryptionType GetCurrentEncryptionType()
		{
			if (cesarRadioButton.Checked)
			{
				return EncryptionType.Cesar;
			}
			else if (textBasedRadioButton.Checked)
			{
				return EncryptionType.TextBased;
			}
			else if (substitutionRadioButton.Checked)
			{
				return EncryptionType.Substitution;
			}
			else if (polybeRadioButton.Checked)
			{
				return EncryptionType.Polybe;
			}
			else if (toBinaryRadioButton.Checked)
			{
				return EncryptionType.ToBinary;
			}
			else
			{
				return EncryptionType.Unknown;
			}
		}

		#region PolybeSetters

		private int currentNumberOfColumns = 0;
		private int columnsWidth = 30;

		private void SetColumns(DataGridView dataGridView, int numberOfColumns)
		{
			dataGridView.Columns.Clear();
			for (int i = 1; i <= numberOfColumns; i++)
			{
				dataGridView.Columns.Add("Column" + currentNumberOfColumns, i.ToString());
				currentNumberOfColumns++;
			}
			foreach (DataGridViewColumn column in dataGridView.Columns)
			{
				column.Width = columnsWidth;
				((DataGridViewTextBoxColumn)column).MaxInputLength = 1;
			}
			for (int i = 0; i <= 3; i++)
			{
				polybeTable.Rows.Add();
			}
		}

		private void RemoveColumn(DataGridView dataGridView, string name)
		{
			dataGridView.Columns.Remove(name);
		}

		private void AddColumn(DataGridView dataGridView, string name)
		{
			dataGridView.Columns.Add(name, (currentNumberOfColumns + 1).ToString());
			dataGridView.Columns[currentNumberOfColumns].Width = 30;
		}

		#endregion PolybeSetters
	}
}