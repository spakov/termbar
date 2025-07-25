using Microsoft.UI.Xaml;
using System;

namespace Spakov.TermBar.Styles
{
    /// <summary>
    /// Style-related helper methods.
    /// </summary>
    internal static class StylesHelper
    {
        /// <summary>
        /// Given a <paramref name="style"/>, sets <paramref name="style"/>'s
        /// <see cref="Style.BasedOn"/> property to the nearest ancestor's
        /// <paramref name="ancestorType"/> <see cref="Style"/>.
        /// </summary>
        /// <remarks>Does nothing to <paramref name="style"/> if a matching
        /// element is not found.</remarks>
        /// <param name="style">The style to modify.</param>
        /// <param name="startAt">The element at which to begin the
        /// search.</param>
        /// <param name="ancestorType">The type of style from which to
        /// inherit.</param>
        /// <returns><see langword="true"/> if a matching style was found or
        /// <see langword="false"/> otherwise.</returns>
        internal static bool MergeWithAncestor(Style style, FrameworkElement startAt, Type ancestorType)
        {
            FrameworkElement? current = startAt;

            object resourcesObject;

            while (current is not null)
            {
                if (
                    current.Resources.TryGetValue(ancestorType, out resourcesObject)
                    && resourcesObject is Style resourcesStyle1
                )
                {
                    style.BasedOn = resourcesStyle1;

                    return true;
                }

                current = current.Parent as FrameworkElement;
            }

            if (
                Application.Current.Resources.TryGetValue(ancestorType, out resourcesObject)
                && resourcesObject is Style resourcesStyle2
            )
            {
                style.BasedOn = resourcesStyle2;

                return true;
            }

            return false;
        }
    }
}