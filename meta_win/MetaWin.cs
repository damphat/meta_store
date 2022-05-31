using meta_store;

namespace meta_win
{
    public partial class MetaWin : Form
    {
        public MetaWin()
        {
            InitializeComponent();
        }

        string Cook(string src)
        {
            try
            {
               var s = Sigo.Parse(src);
                return Sigo.ToString(s);
            } catch (Exception e) {
                return e.Message;
            }
        }

        private void srcTxt_TextChanged(object sender, EventArgs e)
        {
            var src = srcTxt.Text;
            desTxt.Text = Cook(src);
        }
    }
}