﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Retouch_Photo2.Styles
{
    /// <summary>
    /// Provide constant and static methods for XElement.
    /// </summary>
    public static partial class XML
    {
        /// <summary>
        /// Saves the entire <see cref="Style"/>Categorys to a XDocument.
        /// </summary>
        /// <param name="styleCategorys"> The source data. </param>
        /// <returns> The saved XDocument. </returns>
        public static XDocument SaveStyleCategorys(IEnumerable<StyleCategory> styleCategorys)
        {
            return new XDocument
            (
                //Set the document definition for xml.
                new XDeclaration("1.0", "utf-8", "no"),
                new XElement
                (
                    "Root",
                     from style
                     in styleCategorys
                     select XML.SaveStyleCategory("StyleCategory", style)
                )
            );
        }

        /// <summary>
        ///  Loads a <see cref="Style"/>Categorys from an XElement.
        /// </summary>
        /// <param name="document"> The source XDocument. </param>
        /// <returns> The loaded <see cref="Style"/>s. </returns>
        public static IEnumerable<StyleCategory> LoadStyleCategorys(XDocument document)
        {
            XElement root = document.Element("Root");

            return
            (
                from styleCategory
                in root.Elements("StyleCategory")
                select XML.LoadStyleCategory(styleCategory)
            );
        }
        
    }
}