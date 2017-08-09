// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HowToUse.cs" company="MBSoft">
//   MBSoft
// </copyright>
// <summary>
//   Defines the HowToUse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileSearch
{
    using System;
    using System.Windows.Forms;

    public partial class HowToUse : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HowToUse"/> class.
        /// </summary>
        /// <history>
        /// By: Wael Refaat
        /// Email: wael.r.roushdy@gmil.com
        /// On: 9:31 PM
        /// </history>
        public HowToUse()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the btnClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <history>
        /// By: Wael Refaat
        /// Email: wael.r.roushdy@gmil.com
        /// On: 9:31 PM
        /// </history>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
