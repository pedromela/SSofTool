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
			this.Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.LoadButton = new System.Windows.Forms.Button();
			this.File = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.ripLabel = new System.Windows.Forms.Label();
			this.Next = new System.Windows.Forms.Button();
			this.Prev = new System.Windows.Forms.Button();
			this.instruction = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.Stack)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.instruction)).BeginInit();
			this.SuspendLayout();
			// 
			// Code
			// 
			this.Code.Location = new System.Drawing.Point(66, 37);
			this.Code.Name = "Code";
			this.Code.Size = new System.Drawing.Size(295, 318);
			this.Code.TabIndex = 0;
			this.Code.Text = "";
			// 
			// Stack
			// 
			this.Stack.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.Stack.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Address,
            this.Value});
			this.Stack.GridColor = System.Drawing.SystemColors.Control;
			this.Stack.Location = new System.Drawing.Point(462, 37);
			this.Stack.Name = "Stack";
			this.Stack.Size = new System.Drawing.Size(242, 318);
			this.Stack.TabIndex = 1;
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
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(462, 379);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(24, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "rip: ";
			// 
			// ripLabel
			// 
			this.ripLabel.AutoSize = true;
			this.ripLabel.Location = new System.Drawing.Point(493, 379);
			this.ripLabel.Name = "ripLabel";
			this.ripLabel.Size = new System.Drawing.Size(0, 13);
			this.ripLabel.TabIndex = 5;
			// 
			// Next
			// 
			this.Next.Location = new System.Drawing.Point(198, 379);
			this.Next.Name = "Next";
			this.Next.Size = new System.Drawing.Size(43, 23);
			this.Next.TabIndex = 6;
			this.Next.Text = "Next";
			this.Next.UseVisualStyleBackColor = true;
			this.Next.Click += new System.EventHandler(this.Next_Click);
			// 
			// Prev
			// 
			this.Prev.Location = new System.Drawing.Point(152, 379);
			this.Prev.Name = "Prev";
			this.Prev.Size = new System.Drawing.Size(40, 23);
			this.Prev.TabIndex = 7;
			this.Prev.Text = "Prev";
			this.Prev.UseVisualStyleBackColor = true;
			this.Prev.Click += new System.EventHandler(this.Prev_Click);
			// 
			// instruction
			// 
			this.instruction.Location = new System.Drawing.Point(257, 379);
			this.instruction.Name = "instruction";
			this.instruction.Size = new System.Drawing.Size(120, 20);
			this.instruction.TabIndex = 8;
			this.instruction.ValueChanged += new System.EventHandler(this.instruction_ValueChanged);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.instruction);
			this.Controls.Add(this.Prev);
			this.Controls.Add(this.Next);
			this.Controls.Add(this.ripLabel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.File);
			this.Controls.Add(this.LoadButton);
			this.Controls.Add(this.Stack);
			this.Controls.Add(this.Code);
			this.Name = "Form1";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.Stack)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.instruction)).EndInit();
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
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label ripLabel;
		private System.Windows.Forms.Button Next;
		private System.Windows.Forms.Button Prev;
		private System.Windows.Forms.NumericUpDown instruction;
	}
}

