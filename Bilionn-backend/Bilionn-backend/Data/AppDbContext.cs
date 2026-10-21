using Bilionn_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Bilionn_backend.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Items> Items => Set<Items>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("tbl_users");
            e.HasIndex(x => x.UserId).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).IsRequired().HasMaxLength(255);
            e.Property(x => x.PasswordHash).IsRequired().HasMaxLength(100);
            e.Property(x => x.Role).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.AccountStatus).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.ToTable("tbl_category");
            e.HasIndex(x => x.CategoryId).IsUnique();
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Image).HasColumnType("longblob");
        });
        modelBuilder.Entity<Category>(e =>
        {
            e.ToTable("tbl_category");
            e.HasIndex(x => x.CategoryId).IsUnique();
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Image).HasColumnType("longblob");
        });
        modelBuilder.Entity<Items>(e =>
        {
            e.ToTable("tbl_items");
            e.HasIndex(x => x.ItemId).IsUnique();
            e.Property(x => x.Price).HasPrecision(19, 2);
            e.Property(x => x.Image).HasColumnType("longblob");

            e.HasOne(x => x.Category)
             .WithMany()
             .HasForeignKey("CategoryId")
             .IsRequired()
             .OnDelete(DeleteBehavior.Restrict);
            e.Property<long>("CategoryId").HasColumnName("category_id");
        });
        modelBuilder.Entity<Employee>(e =>
        {
            e.ToTable("tbl_employees");
            e.HasIndex(x => x.EmployeeCode).IsUnique().HasDatabaseName("uk_employee_code");
            e.HasIndex(x => x.PanNumber).IsUnique().HasDatabaseName("uk_pan_number");
            e.HasIndex(x => x.AadhaarNumber).IsUnique().HasDatabaseName("uk_aadhaar_number");

            e.Property(x => x.EmployeeCode).IsRequired().HasMaxLength(30);
            e.Property(x => x.FullName).IsRequired().HasMaxLength(100);
            e.Property(x => x.MobileNumber).IsRequired().HasMaxLength(15);
            e.Property(x => x.PanNumber).HasMaxLength(10);
            e.Property(x => x.AadhaarNumber).HasMaxLength(12);
            e.Property(x => x.Salary).HasPrecision(12, 2);

            e.OwnsOne(x => x.BankDetails);

            e.OwnsOne(x => x.EmergencyContact, ec =>
            {
                ec.Property(p => p.Name).HasColumnName("emergency_contact_name").HasMaxLength(100);
                ec.Property(p => p.MobileNumber).HasColumnName("emergency_contact_mobile_number").HasMaxLength(15);
                ec.Property(p => p.Relationship).HasColumnName("emergency_contact_relationship").HasMaxLength(50);
            });

            // @OneToOne(optional = false) + unique user_account_id
            e.HasOne(x => x.UserAccount)
             .WithOne(u => u.Employee)
             .HasForeignKey<Employee>(x => x.UserAccountId)
             .IsRequired()
             .OnDelete(DeleteBehavior.Restrict);
            // @ManyToOne created_by_admin_id
            e.HasOne(x => x.CreatedByAdmin)
             .WithMany()
             .HasForeignKey(x => x.CreatedByAdminId)
             .IsRequired()
             .OnDelete(DeleteBehavior.Restrict);

            e.Property(x => x.UserAccountId).HasColumnName("user_account_id");
            e.Property(x => x.CreatedByAdminId).HasColumnName("created_by_admin_id");

            e.OwnsOne(x => x.Address, a =>
            {
                a.Property(p => p.HouseNo).HasColumnName("house_no").HasMaxLength(50);
                a.Property(p => p.Street).HasColumnName("street").HasMaxLength(150);
                a.Property(p => p.Area).HasColumnName("area").HasMaxLength(150);
                a.Property(p => p.City).HasColumnName("city").HasMaxLength(100);
                a.Property(p => p.State).HasColumnName("state").HasMaxLength(100);
                a.Property(p => p.Pincode).HasColumnName("pincode").HasMaxLength(10);
            });
        });
        modelBuilder.Entity<Order>(e =>
        {
            e.ToTable("tbl_orders");
            e.Property(x => x.Subtotal).HasPrecision(19, 2);
            e.Property(x => x.Tax).HasPrecision(19, 2);
            e.Property(x => x.GrandTotal).HasPrecision(19, 2);

            // cascade = ALL + orphanRemoval, unidirectional with order_id column
            e.HasMany(x => x.Items)
             .WithOne()
             .HasForeignKey("OrderId")
             .IsRequired()
             .OnDelete(DeleteBehavior.Cascade);

            e.Property(x => x.PaymentMethod).HasConversion<string>().HasMaxLength(20);

            e.OwnsOne(x => x.PaymentDetails, p =>
            {
                p.Property(x => x.RazorpayOrderId).HasColumnName("razorpay_order_id");
                p.Property(x => x.RazorpayPaymentId).HasColumnName("razorpay_payment_id");
                p.Property(x => x.RazorpaySignature).HasColumnName("razorpay_signature");
                p.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20);
            });
        });

        modelBuilder.Entity<OrderItem>(e =>
        {
            e.ToTable("tbl_order_items");
            e.Property(x => x.Price).HasPrecision(19, 2);
            e.Property<long>("OrderId").HasColumnName("order_id");
        });

    }

    public override int SaveChanges()
    {
        ApplyAuditRules();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditRules();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditRules()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            // 1. Agar koi entity IAuditable implement karti hai:
            if (entry.Entity is IAuditable auditable)
            {
                if (entry.State == EntityState.Added)
                {
                    auditable.CreatedAt = now;
                    auditable.UpdatedAt = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    auditable.UpdatedAt = now;
                    entry.Property(nameof(IAuditable.CreatedAt)).IsModified = false; // updatable = false
                }
            }
            // 2. Agar naya Order ban raha hai, toh automatic custom Order ID generate hogi
            if (entry.Entity is Order order && entry.State == EntityState.Added)
            {
                order.OrderId = "ORD" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                order.CreatedAt = now;
            }
        }
    }



        // DbSet<Product> Products => we'll add after we define entities......
    }
