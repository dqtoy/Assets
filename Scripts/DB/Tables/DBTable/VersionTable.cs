﻿
using ManualTable.Row;
using UnityEngine;

namespace ManualTable
{
    [CreateAssetMenu(fileName = "New Version Table", menuName = "SQLiteTable/Version", order = 3)]
    public class VersionTable : ManualTableBase<VersionRow> { }
}