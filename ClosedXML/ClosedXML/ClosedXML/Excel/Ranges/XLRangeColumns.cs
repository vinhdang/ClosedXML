﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClosedXML.Excel
{
    internal class XLRangeColumns : IXLRangeColumns
    {
        XLWorksheet worksheet;
        public XLRangeColumns(XLWorksheet worksheet)
        {
            Style = worksheet.Style;
            this.worksheet = worksheet;
        }

        List<XLRangeColumn> ranges = new List<XLRangeColumn>();

        public void Clear()
        {
            ranges.ForEach(r => r.Clear());
        }

        public void Delete()
        {
            ranges.OrderByDescending(c => c.ColumnNumber()).ForEach(r => r.Delete());
            ranges.Clear();
        }

        public void Add(IXLRangeColumn range)
        {
            ranges.Add((XLRangeColumn)range);
        }

        public IEnumerator<IXLRangeColumn> GetEnumerator()
        {
            var retList = new List<IXLRangeColumn>();
            ranges.ForEach(c => retList.Add(c));
            return retList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region IXLStylized Members

        private IXLStyle style;
        public IXLStyle Style
        {
            get
            {
                return style;
            }
            set
            {
                style = new XLStyle(this, value);
                ranges.ForEach(r => r.Style = value);
            }
        }

        public IEnumerable<IXLStyle> Styles
        {
            get
            {
                UpdatingStyle = true;
                yield return style;
                foreach (var rng in ranges)
                {
                    yield return rng.Style;
                    foreach (var r in rng.Worksheet.Internals.CellsCollection.Values.Where(c =>
                        c.Address.RowNumber >= rng.RangeAddress.FirstAddress.RowNumber
                        && c.Address.RowNumber <= rng.RangeAddress.LastAddress.RowNumber
                        && c.Address.ColumnNumber >= rng.RangeAddress.FirstAddress.ColumnNumber
                        && c.Address.ColumnNumber <= rng.RangeAddress.LastAddress.ColumnNumber
                        ))
                    {
                        yield return r.Style;
                    }
                }
                UpdatingStyle = false;
            }
        }

        public Boolean UpdatingStyle { get; set; }

        #endregion


        public IXLCells Cells()
        {
            var cellHash = new HashSet<IXLCell>();
            foreach (var container in ranges)
            {
                foreach (var cell in container.Cells())
                {
                    if (!cellHash.Contains(cell))
                    {
                        cellHash.Add(cell);
                    }
                }
            }
            var cells = new XLCells(worksheet);
            cells.AddRange(cellHash);
            return (IXLCells)cells;
        }

        public IXLCells CellsUsed()
        {
            var cellHash = new HashSet<IXLCell>();
            foreach (var container in ranges)
            {
                foreach (var cell in container.CellsUsed())
                {
                    if (!cellHash.Contains(cell))
                    {
                        cellHash.Add(cell);
                    }
                }
            }
            var cells = new XLCells(worksheet);
            cells.AddRange(cellHash);
            return (IXLCells)cells;
        }

        public IXLCells CellsUsed(Boolean includeStyles)
        {
            var cellHash = new HashSet<IXLCell>();
            foreach (var container in ranges)
            {
                foreach (var cell in container.CellsUsed(includeStyles))
                {
                    if (!cellHash.Contains(cell))
                    {
                        cellHash.Add(cell);
                    }
                }
            }
            var cells = new XLCells(worksheet);
            cells.AddRange(cellHash);
            return (IXLCells)cells;
        }
    }
}
