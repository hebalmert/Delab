using Delab.Shared.EntitesSoftSec;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delab.AccessData.ModelConfig.EntitesSoftSec;

public class UsuarioConfig : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(e => e.UsuarioId);
        builder.HasIndex(e => e.UserName).IsUnique();
        builder.HasIndex(x => new { x.FullName, x.Nro_Document, x.CorporationId }).IsUnique();
        //Evitar el borrado en cascada
        builder.HasOne(e => e.Corporation).WithMany(c => c.Usuarios).OnDelete(DeleteBehavior.Restrict);
    }
}