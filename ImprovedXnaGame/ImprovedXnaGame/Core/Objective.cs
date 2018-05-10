namespace Age.Core
{
    class Objective : Trigger
    {
        public bool Complete { get; set; }
        public bool Visible { get; set; }
        public string Caption { get; set; }

        public Objective(string caption)
        {
            Complete = false;
            Visible = true;
            Caption = caption;
        }
    }
}