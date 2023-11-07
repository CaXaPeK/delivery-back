using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Delivery_Service.Context;

public partial class DeliveryDbContext : DbContext
{
    public DeliveryDbContext()
    {
    }

    public DeliveryDbContext(DbContextOptions<DeliveryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AsAddrObj> AsAddrObjs { get; set; }

    public virtual DbSet<AsAdmHierarchy> AsAdmHierarchies { get; set; }

    public virtual DbSet<AsHouse> AsHouses { get; set; }

    public virtual DbSet<Dish> Dishes { get; set; }

    public virtual DbSet<DishInCart> DishInCarts { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Gar70;Username=postgres;Password=sasha220304");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AsAddrObj>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Addr_Objs");

            entity.ToTable("as_addr_obj", "fias", tb => tb.HasComment("Сведения классификатора адресообразующих элементов"));

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("Уникальный идентификатор записи. Ключевое поле")
                .HasColumnName("id");
            entity.Property(e => e.Changeid)
                .HasComment("ID изменившей транзакции")
                .HasColumnName("changeid");
            entity.Property(e => e.Enddate)
                .HasComment("Окончание действия записи")
                .HasColumnName("enddate");
            entity.Property(e => e.Isactive)
                .HasComment("Признак действующего адресного объекта")
                .HasColumnName("isactive");
            entity.Property(e => e.Isactual)
                .HasComment("Статус актуальности адресного объекта ФИАС")
                .HasColumnName("isactual");
            entity.Property(e => e.Level)
                .HasComment("Уровень адресного объекта")
                .HasColumnName("level");
            entity.Property(e => e.Name)
                .HasComment("Наименование")
                .HasColumnName("name");
            entity.Property(e => e.Nextid)
                .HasComment("Идентификатор записи связывания с последующей исторической записью")
                .HasColumnName("nextid");
            entity.Property(e => e.Objectguid)
                .HasComment("Глобальный уникальный идентификатор адресного объекта типа UUID")
                .HasColumnName("objectguid");
            entity.Property(e => e.Objectid)
                .HasComment("Глобальный уникальный идентификатор адресного объекта типа INTEGER")
                .HasColumnName("objectid");
            entity.Property(e => e.Opertypeid)
                .HasComment("Статус действия над записью – причина появления записи")
                .HasColumnName("opertypeid");
            entity.Property(e => e.Previd)
                .HasComment("Идентификатор записи связывания с предыдущей исторической записью")
                .HasColumnName("previd");
            entity.Property(e => e.Startdate)
                .HasComment("Начало действия записи")
                .HasColumnName("startdate");
            entity.Property(e => e.Typename)
                .HasComment("Краткое наименование типа объекта")
                .HasColumnName("typename");
            entity.Property(e => e.Updatedate)
                .HasComment("Дата внесения (обновления) записи")
                .HasColumnName("updatedate");
        });

        modelBuilder.Entity<AsAdmHierarchy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Adm_Hier");

            entity.ToTable("as_adm_hierarchy", "fias", tb => tb.HasComment("Сведения по иерархии в административном делении"));

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("Уникальный идентификатор записи. Ключевое поле")
                .HasColumnName("id");
            entity.Property(e => e.Areacode)
                .HasComment("Код района")
                .HasColumnName("areacode");
            entity.Property(e => e.Changeid)
                .HasComment("ID изменившей транзакции")
                .HasColumnName("changeid");
            entity.Property(e => e.Citycode)
                .HasComment("Код города")
                .HasColumnName("citycode");
            entity.Property(e => e.Enddate)
                .HasComment("Окончание действия записи")
                .HasColumnName("enddate");
            entity.Property(e => e.Isactive)
                .HasComment("Признак действующего адресного объекта")
                .HasColumnName("isactive");
            entity.Property(e => e.Nextid)
                .HasComment("Идентификатор записи связывания с последующей исторической записью")
                .HasColumnName("nextid");
            entity.Property(e => e.Objectid)
                .HasComment("Глобальный уникальный идентификатор объекта")
                .HasColumnName("objectid");
            entity.Property(e => e.Parentobjid)
                .HasComment("Идентификатор родительского объекта")
                .HasColumnName("parentobjid");
            entity.Property(e => e.Path)
                .HasComment("Материализованный путь к объекту (полная иерархия)")
                .HasColumnName("path");
            entity.Property(e => e.Placecode)
                .HasComment("Код населенного пункта")
                .HasColumnName("placecode");
            entity.Property(e => e.Plancode)
                .HasComment("Код ЭПС")
                .HasColumnName("plancode");
            entity.Property(e => e.Previd)
                .HasComment("Идентификатор записи связывания с предыдущей исторической записью")
                .HasColumnName("previd");
            entity.Property(e => e.Regioncode)
                .HasComment("Код региона")
                .HasColumnName("regioncode");
            entity.Property(e => e.Startdate)
                .HasComment("Начало действия записи")
                .HasColumnName("startdate");
            entity.Property(e => e.Streetcode)
                .HasComment("Код улицы")
                .HasColumnName("streetcode");
            entity.Property(e => e.Updatedate)
                .HasComment("Дата внесения (обновления) записи")
                .HasColumnName("updatedate");
        });

        modelBuilder.Entity<AsHouse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Houses");

            entity.ToTable("as_houses", "fias", tb => tb.HasComment("Сведения по номерам домов улиц городов и населенных пунктов"));

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("Уникальный идентификатор записи. Ключевое поле")
                .HasColumnName("id");
            entity.Property(e => e.Addnum1)
                .HasComment("Дополнительный номер дома 1")
                .HasColumnName("addnum1");
            entity.Property(e => e.Addnum2)
                .HasComment("Дополнительный номер дома 1")
                .HasColumnName("addnum2");
            entity.Property(e => e.Addtype1)
                .HasComment("Дополнительный тип дома 1")
                .HasColumnName("addtype1");
            entity.Property(e => e.Addtype2)
                .HasComment("Дополнительный тип дома 2")
                .HasColumnName("addtype2");
            entity.Property(e => e.Changeid)
                .HasComment("ID изменившей транзакции")
                .HasColumnName("changeid");
            entity.Property(e => e.Enddate)
                .HasComment("Окончание действия записи")
                .HasColumnName("enddate");
            entity.Property(e => e.Housenum)
                .HasComment("Основной номер дома")
                .HasColumnName("housenum");
            entity.Property(e => e.Housetype)
                .HasComment("Основной тип дома")
                .HasColumnName("housetype");
            entity.Property(e => e.Isactive)
                .HasComment("Признак действующего адресного объекта")
                .HasColumnName("isactive");
            entity.Property(e => e.Isactual)
                .HasComment("Статус актуальности адресного объекта ФИАС")
                .HasColumnName("isactual");
            entity.Property(e => e.Nextid)
                .HasComment("Идентификатор записи связывания с последующей исторической записью")
                .HasColumnName("nextid");
            entity.Property(e => e.Objectguid)
                .HasComment("Глобальный уникальный идентификатор адресного объекта типа UUID")
                .HasColumnName("objectguid");
            entity.Property(e => e.Objectid)
                .HasComment("Глобальный уникальный идентификатор объекта типа INTEGER")
                .HasColumnName("objectid");
            entity.Property(e => e.Opertypeid)
                .HasComment("Статус действия над записью – причина появления записи")
                .HasColumnName("opertypeid");
            entity.Property(e => e.Previd)
                .HasComment("Идентификатор записи связывания с предыдущей исторической записью")
                .HasColumnName("previd");
            entity.Property(e => e.Startdate)
                .HasComment("Начало действия записи")
                .HasColumnName("startdate");
            entity.Property(e => e.Updatedate)
                .HasComment("Дата внесения (обновления) записи")
                .HasColumnName("updatedate");
        });

        modelBuilder.Entity<Dish>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("dish_pkey");

            entity.ToTable("dish", "fias");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsVegetarian).HasColumnName("isVegetarian");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Photo).HasColumnName("photo");
            entity.Property(e => e.Price).HasColumnName("price");
        });

        modelBuilder.Entity<DishInCart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("dishInCart_pkey");

            entity.ToTable("dishInCart", "fias");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.DishId).HasColumnName("dishId");
            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.UserId).HasColumnName("userId");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_pkey");

            entity.ToTable("order", "fias");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AddressId).HasColumnName("addressId");
            entity.Property(e => e.DeliveryDate).HasColumnName("deliveryDate");
            entity.Property(e => e.DeliveryTime).HasColumnName("deliveryTime");
            entity.Property(e => e.OrderDate).HasColumnName("orderDate");
            entity.Property(e => e.OrderTime).HasColumnName("orderTime");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("order_userId_fkey");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("rating_pkey");

            entity.ToTable("rating", "fias");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.DishId).HasColumnName("dishId");
            entity.Property(e => e.Rating1).HasColumnName("rating");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Dish).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.DishId)
                .HasConstraintName("rating_dishId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("rating_userId_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pkey");

            entity.ToTable("user", "fias");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.BirthDate).HasColumnName("birthDate");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FullName).HasColumnName("fullName");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Phone).HasColumnName("phone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
