using System.Collections.Generic;

public interface IHexOwner
{
    /// <summary>
    /// Territorial draft object
    /// </summary>
    Dictionary<Hex, Biom> BottomDraft { get; }
}