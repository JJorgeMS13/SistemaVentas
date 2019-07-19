
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sistema.Entidades.Almacen;

namespace Sistema.Datos.Mapping.Almacen
{
    public class CategoriaMap : IEntityTypeConfiguration<Categoria>
    {
        //Se hace esto para mapear la base de datos con EntityFramework
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            //Se le dice a que tabla se va a Mappear en este caso "categoria" y se le dice cual es la llave primaria 
            builder.ToTable("categoria")
                .HasKey(c => c.idcategoria);
            builder.Property(c => c.nombre)
                .HasMaxLength(50);
            builder.Property(c => c.descripcion)
                .HasMaxLength(256);
        }
    }
}
