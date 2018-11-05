namespace SSofTool
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Code = new System.Windows.Forms.RichTextBox();
			this.Stack = new System.Windows.Forms.DataGridView();
			this.LoadButton = new System.Windows.Forms.Button();
			this.File = new System.Windows.Forms.TextBox();
			this.Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.Stack)).BeginInit();
			this.SuspendLayout();
			// 
			// Code
			// 
			this.Code.Location = new System.Drawing.Point(66, 37);
			this.Code.Name = "Code";
			this.Code.Size = new System.Drawing.Size(175, 318);
			this.Code.TabIndex = 0;
			this.Code.Text = "";
			// 
			// Stack
			// 
			this.Stack.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.Stack.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Address,
            this.Value});
			this.Stack.Location = new System.Drawing.Point(385, 37);
			this.Stack.Name = "Stack";
			this.Stack.Size = new System.Drawing.Size(254, 318);
			this.Stack.TabIndex = 1;
			// 
			// LoadButton
			// 
			this.LoadButton.Location = new System.Drawing.Point(66, 379);
			this.LoadButton.Name = "LoadButton";
			this.LoadButton.Size = new System.Drawing.Size(75, 23);
			this.LoadButton.TabIndex = 2;
			this.LoadButton.Text = "Load";
			this.LoadButton.UseVisualStyleBackColor = true;
			this.LoadButton.Click += new System.EventHandler(this.LoadButton_Click);
			// 
			// File
			// 
			this.File.Location = new System.Drawing.Point(66, 408);
			this.File.Name = "File";
			this.File.Size = new System.Drawing.Size(175, 20);
			this.File.TabIndex = 3;
			// 
			// Address
			// 
			this.Address.HeaderText = "Address";
			this.Address.Name = "Address";
			// 
			// Value
			// 
			this.Value.HeaderText = "Value";
			this.Value.Name = "Value";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.File);
			this.Controls.Add(this.LoadButton);
			this.Controls.Add(this.Stack);
			this.Controls.Add(this.Code);
			this.Name = "Form1";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.Stack)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RichTextBox Code;
		private System.Windows.Forms.DataGridView Stack;
		private System.Windows.Forms.Button LoadButton;
		private System.Windows.Forms.TextBox File;
		private System.Windows.Forms.DataGridViewTextBoxColumn Address;
		private System.Windows.Forms.DataGridViewTextBoxColumn Value;
	}
}

