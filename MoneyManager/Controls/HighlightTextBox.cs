using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MoneyManager.Controls
{

    public class HighlightTextBlock : TextBlock
    {

        #region Constructors

        public HighlightTextBlock()
            : base()
        {
            DefaultStyleKey = typeof(HighlightTextBlock);
            InitializeBinding();
        }

        public HighlightTextBlock(Inline inline)
            : base(inline)
        {
            DefaultStyleKey = typeof(HighlightTextBlock);
            InitializeBinding();
        }

        #endregion

        #region Properties

        public string MyText
        {
            get { return (string)GetValue(MyTextProperty); }
            set { SetValue(MyTextProperty, value); }
        }


        public static readonly DependencyProperty MyTextProperty =
            DependencyProperty.Register("MyText", typeof(string), typeof(HighlightTextBlock), new UIPropertyMetadata(OnMyTextChanged));

        public string HighlightText
        {
            get { return (string)GetValue(HighlightTextProperty); }
            set { SetValue(HighlightTextProperty, value); }
        }

        public static readonly DependencyProperty HighlightTextProperty =
            DependencyProperty.Register("HighlightText", typeof(string), typeof(HighlightTextBlock), new UIPropertyMetadata(OnHighlightTextChanged));

        public Brush HighlightBrush
        {
            get { return (Brush)GetValue(HighlightBrushProperty); }
            set { SetValue(HighlightBrushProperty, value); }
        }

        public static readonly DependencyProperty HighlightBrushProperty =
            DependencyProperty.Register("HighlightBrush", typeof(Brush), typeof(HighlightTextBlock), new UIPropertyMetadata(null, OnHighlightBrushChanged));

        public FontWeight HighlightFontWeight
        {
            get { return (FontWeight)GetValue(HighlightFontWeightProperty); }
            set { SetValue(HighlightFontWeightProperty, value); }
        }

        public static readonly DependencyProperty HighlightFontWeightProperty =
            DependencyProperty.Register("HighlightFontWeight", typeof(FontWeight), typeof(HighlightTextBlock), new UIPropertyMetadata(FontWeights.Normal, OnHighlightFontWeightChanged));

        #endregion

        #region Methods

        static void OnMyTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            HighlightTextBlock source = obj as HighlightTextBlock;

            source.Inlines.Clear();

            string text = e.NewValue as string;

            if (!string.IsNullOrEmpty(text))
            {
                for (int i = 0; i <= text.Length - 1; i++)
                {
                    Inline run = new Run { Text = text[i].ToString() };
                    source.Inlines.Add(run);
                }

                source.ApplyHighlighting();
            }
        }

        static void OnHighlightTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            HighlightTextBlock source = obj as HighlightTextBlock;
            source.ApplyHighlighting();
        }

        static void OnHighlightBrushChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            HighlightTextBlock source = obj as HighlightTextBlock;
            source.ApplyHighlighting();
        }

        static void OnHighlightFontWeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            HighlightTextBlock source = obj as HighlightTextBlock;
            //FontWeight value = (FontWeight)e.NewValue;
            source.ApplyHighlighting();
        }

        void InitializeBinding()
        {
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Text");
            this.SetBinding(HighlightTextBlock.MyTextProperty, binding);
        }

        void ApplyHighlighting()
        {
            if (Inlines == null)
                return;

            string text = this.Text ?? string.Empty;
            string highlight = this.HighlightText ?? string.Empty;
            StringComparison compare = StringComparison.OrdinalIgnoreCase;

            int cur = 0;

            while (cur < text.Length)
            {
                int i = highlight.Length == 0 ? -1 : text.IndexOf(highlight, cur, compare);
                i = i < 0 ? text.Length : i;

                //clear
                while (cur < i && cur < text.Length)
                {
                    Inlines.ElementAt(cur).Foreground = Foreground;
                    Inlines.ElementAt(cur).FontWeight = FontWeight;
                    cur++;
                }

                //highlight
                int start = cur;

                while (cur < start + highlight.Length && cur < text.Length)
                {
                    Inlines.ElementAt(cur).Foreground = HighlightBrush;
                    Inlines.ElementAt(cur).FontWeight = HighlightFontWeight;
                    cur++;
                }
            }
        }

        #endregion

    }
}
