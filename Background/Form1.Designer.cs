namespace Background
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
            
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.rtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.thyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sdfgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dsgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.rtToolStripMenuItem,
            this.thyToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 92);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sdfgToolStripMenuItem,
            this.dsgToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem1.Text = "qwe";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // rtToolStripMenuItem
            // 
            this.rtToolStripMenuItem.Name = "rtToolStripMenuItem";
            this.rtToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.rtToolStripMenuItem.Text = "rt";
            // 
            // thyToolStripMenuItem
            // 
            this.thyToolStripMenuItem.Name = "thyToolStripMenuItem";
            this.thyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.thyToolStripMenuItem.Text = "thy";
            // 
            // sdfgToolStripMenuItem
            // 
            this.sdfgToolStripMenuItem.Name = "sdfgToolStripMenuItem";
            this.sdfgToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.sdfgToolStripMenuItem.Text = "sdfg";
            // 
            // dsgToolStripMenuItem
            // 
            this.dsgToolStripMenuItem.Name = "dsgToolStripMenuItem";
            this.dsgToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.dsgToolStripMenuItem.Text = "dsg";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "Form1";
            this.Text = "Form1";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem sdfgToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dsgToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rtToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem thyToolStripMenuItem;
    }
}