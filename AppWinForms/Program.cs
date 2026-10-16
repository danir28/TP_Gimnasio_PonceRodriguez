using AccesoDatos.Data;
using Microsoft.EntityFrameworkCore;

namespace AppWinForms;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var context = new AplicacionDbContext();

        try
        {
            // Crea la base de datos (Gym.db) y aplica las migraciones si hace falta.
            // Así no hace falta correr "database update" en cada computadora.
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo preparar la base de datos:\n{ex.Message}",
                "Gimnasio",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        Application.Run(new FormSocios(context));
    }
}