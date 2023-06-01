using Microsoft.EntityFrameworkCore;

public class JiraTaskDb : DbContext
{
	public JiraTaskDb(DbContextOptions<JiraTaskDb> options)
		: base(options) { }

	public DbSet<JiraTask> JiraTasks => Set<JiraTask>();
}
