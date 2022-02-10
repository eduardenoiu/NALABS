using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Dynamic;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using RCM.Metrics;

namespace RCM
{
    public  class Requirement :  INotifyPropertyChanged
    {
        //private string id;
        private static ObservableCollection<Requirement> allRequirements=new ObservableCollection<Requirement>();

        internal static ObservableCollection<Requirement> AllRequirements
        {
            get { return Requirement.allRequirements; }
            set
            {
                Requirement.allRequirements = new ObservableCollection<Requirement>();
                foreach (Requirement req in value)
                    allRequirements.Add(req);
            }
        }

        //public string Id
        //{
        //    get { return id; }
        //    set { id = value; }
        //}
        private string text;

        private FlowDocument doc;

        public FlowDocument Doc
        {
            get { return doc; }
            set
            {
                doc = value;
            }
        }
        //public string Text
        //{
        //    get { return text; }
        //    set { text = value; }
        //}

        public static void Update()
        {
            Parallel.ForEach(AllRequirements, r =>
            {
                r.Text = r.text;
            });
        }
        public string Id { get; set; }
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value.Trim();
                this.Words = Text.Trim().Split(new char[] { ' ', '\n' }).Where(n => !string.IsNullOrEmpty(n) && n != " ").ToArray();
                this.WordNumber = Words.Count();
                Run ru = new Run(Text);

                Metric m = MetricManager.Instance.AllMetrics.Find(p => p.Instance is ConjunctionsMetric);                
                Metric nv = MetricManager.Instance.AllMetrics.Find(p => p.Instance is NVMetric);
                Metric op = MetricManager.Instance.AllMetrics.Find(p => p.Instance is OptionalityMetric);
                Metric sp = MetricManager.Instance.AllMetrics.Find(p => p.Instance is SubjectivityMetric) ;
                Metric nr = MetricManager.Instance.AllMetrics.Find(p => p.Instance is ReferencesMetric) ;
                Metric we = MetricManager.Instance.AllMetrics.Find(p => p.Instance is WeaknessMetric) ;
                Metric im = MetricManager.Instance.AllMetrics.Find(p => p.Instance is ImperativesMetric);
                Metric con = MetricManager.Instance.AllMetrics.Find(p => p.Instance is ContinuancesMetric);
                Metric im2 = MetricManager.Instance.AllMetrics.Find(p => p.Instance is Imperatives2);
                Metric ref2 = MetricManager.Instance.AllMetrics.Find(p => p.Instance is References2);

                TextPointer pointer = ru.ContentStart;
                int conj=0;
                int vag = 0;
                int opt = 0;
                int sb = 0;
                int refs = 0;
                int weak = 0;
                int imp = 0;
                int cont = 0;
                int imm2 = 0;
                int reff2 = 0;

                IEnumerable<TextRange> ranges = MainWindow.GetAllWordRanges(ru);

                foreach(TextRange range in ranges)
                {
                    if (m.Instance.Keywords.AsEnumerable().Contains(range.Text.ToLower()))
                        conj++;
                    if (nv.Instance.Keywords.AsEnumerable().Contains(range.Text.ToLower()))
                        vag++;
                    if (op.Instance.Keywords.AsEnumerable().Contains(range.Text.ToLower()))
                        opt++;
                    if (sp.Instance.Keywords.AsEnumerable().Contains(range.Text.ToLower()))
                        sb++;
                    if (nr.Instance.Keywords.AsEnumerable().Contains(range.Text.ToLower()))
                        refs++;
                    if (we.Instance.Keywords.AsEnumerable().Contains(range.Text.ToLower()))
                        weak++;
                    if (im.Instance.Keywords.Contains(range.Text.ToLower()))
                        imp++;
                    if (con.Instance.Keywords.Contains(range.Text.ToLower()))
                        cont++;
                    if (im2.Instance.Keywords.Contains(range.Text.ToLower()))
                        imm2++;
                    if (ref2.Instance.Keywords.Contains(range.Text.ToLower()))
                        reff2++;
                }

                this.Conjunctions = conj;
                this.VaguePhrases = vag;
                this.Optionality = opt;
                this.Subjectivity = sb;
                this.References = refs;
                this.Weakness = weak;
                this.Imperatives = imp;
                this.Continuances = cont;
                this.Imperatives2 = imm2;
                this.References2 = reff2;
             
                this.Sentences = Text.Trim().Split(new char[] { '.' }).Where(n => !string.IsNullOrEmpty(n) && n != " " && n.Length > 1).ToArray();
                double sum = 0;
                foreach (string s in this.Sentences)
                {
                    sum += s.Split(' ').Length;
                }
                double wordsum = 0;
                foreach (string s in this.Words)
                {
                    wordsum += s.Length;
                }
                this.ARI = sum / this.Sentences.Length + 9 * (wordsum / this.Words.Count());
                NotifyPropertyChanged("Text");
                NotifyPropertyChanged("WordNumber");
                NotifyPropertyChanged("Conjunctions");
                NotifyPropertyChanged("VaguePhrases");
                NotifyPropertyChanged("Optionality");
                NotifyPropertyChanged("Subjectivity");
                NotifyPropertyChanged("References");
                NotifyPropertyChanged("Weakness");
                NotifyPropertyChanged("ARI");
            }
        }

        public double ARI { get; set; }
        public string[] Sentences { get; set; }
        public string[] Words { get; set; }
        public int WordNumber { get; set; }
        public int Conjunctions { get; set;}
        public int VaguePhrases { get; set; }
        public int Optionality { get; set; }
        public int Subjectivity { get; set; }
        public int References { get; set; }
        public int Weakness { get; set; }
        public int Imperatives { get; set; }
        public int Continuances { get; set; }
        public int Imperatives2 { get; set; }
        public int References2 { get; set; }

        public Requirement(string id, string text)
        {
            this.Id = id;
            this.Text = text.Trim(' ', '\n', '\r', '\t');
            //    this.Words = text.Trim().Split(new char[] { ' ', '\n' });
            //    this.WordNumber = Words.Count();
            //    this.Conjuctions = Words.Count(f => ((ConjuctionsMetric)MainWindow.UsedMetrics.Find(s => s is ConjuctionsMetric)).Keywords.Contains(f));
        }

        public Requirement()
        {
            
        }

        public override string ToString()
        {
            return Id + " " + Text;
        }

        //public void RecalculateMetrics()
        //{
        //    this.Words = Text.Split(new char[] { ' ', '\n' });
        //    this.WordNumber = Words.Count();
        //    this.Conjuctions = Words.Count(f => ((ConjuctionsMetric)MainWindow.UsedMetrics.Find(s => s is ConjuctionsMetric)).Keywords.Contains(f));
        //}        


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName)); //  .Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
