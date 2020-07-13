﻿using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace BattleShip.WPF.Models
{
    public static class RichTextBoxExtension
    {
        public static void AppendText(this RichTextBox box, string text, Brush brush)
        {
            box.Dispatcher.Invoke(new Action(() =>
            {
                TextRange range = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
                range.Text = text + '\n';
                range.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
                box.ScrollToEnd();
            }));
        }

        public static void AppendText(this RichTextBox box, Exception ex)
        {
            box.Dispatcher.Invoke(new Action(() =>
            {
                TextRange range = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
                range.Text = ex.Message + '\n';
                range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                box.ScrollToEnd();
            }));
        }
    }
}
