using RCM.Helpers;
using System;
using System.Windows;

public static class MessageHelper
{
    public static void ShowWarning(Exception ex, string context = "", string caption = "")
    {
        var errorMessage = $"An unexpected error occurred."
                    + $"{(string.IsNullOrEmpty(context) ? "" : "\n\nContext: " + context)}"
                    + $"{(string.IsNullOrEmpty(ex.Message) ? "" : "\n\nMessage: " + ex.Message)}"
                    + $"{(string.IsNullOrEmpty(ex.StackTrace) ? "" : "\n\nStackTrace: " + ex.StackTrace)}";

        if (EnvironmentContext.IsCI)
        {
            Console.Error.WriteLine(errorMessage);
        }
        else
        {
            ShowWarning(context, caption);
        }
    }

    public static void Show(string messageBoxText)
    {
        if (EnvironmentContext.IsCI)
        {
            Console.Error.WriteLine(messageBoxText);
        }
        else
        {
            MessageBox.Show(messageBoxText);
        }
    }

    public static void ShowError(string errorMessage, string caption = "Error")
    {
        if (EnvironmentContext.IsCI)
        {
            Console.Error.WriteLine(errorMessage);
        }
        else
        {
            MessageBox.Show(errorMessage, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static void ShowWarning(string errorMessage, string caption = "Warning")
    {
        if (EnvironmentContext.IsCI)
        {
            Console.Error.WriteLine(errorMessage);
        }
        else
        {
            MessageBox.Show(errorMessage, caption, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
