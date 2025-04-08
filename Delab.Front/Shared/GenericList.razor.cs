using Delab.Shared.Entities;
using Microsoft.AspNetCore.Components;

namespace Delab.Front.Shared;

public partial class GenericList<Titem>
{
    [Parameter] public RenderFragment? Loading { get; set; }

    [Parameter] public RenderFragment? NoRecords { get; set; }

    [Parameter, EditorRequired] public List<Titem> MyList { get; set; } = null!;

    [Parameter, EditorRequired] public RenderFragment Body { get; set; } = null!;
}