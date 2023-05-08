using System;

namespace Ex03_PavimentacionCalles
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Programa para simular la pavimentación de tramos de calle");
            Console.WriteLine("Las calles tendrán una longitud entre 100 y 500 mts");
            Console.WriteLine("Los tramos deteriorados tienen una longitud relativa entre el 20 y 60%");
            Console.WriteLine("\nSe repararán calles por los siguientes deterioros");

            string[] losDeterioros = { "Agrietamientos", "Hundimientos", "Ondulaciones" };

            foreach (string deterioro in losDeterioros)
                Console.WriteLine($"- {deterioro}");

            //1. Obtener la cantidad de calles a pavimentar
            int cantidadCalles = 0;
            bool datoCorrecto = false;

            do
            {
                try
                {
                    Console.Write("\nIngresa la cantidad de calles a pavimentar: ");
                    cantidadCalles = int.Parse(Console.ReadLine()!);

                    if (cantidadCalles > 0)
                        datoCorrecto = true;
                    else
                        Console.WriteLine("La cantidad de calles debe ser un entero positivo. Intenta nuevamente!");
                }
                catch (FormatException unError)
                {
                    Console.WriteLine("La cantidad de calles debe ser un entero positivo. Intenta nuevamente");
                    Console.WriteLine(unError.Message);
                }
            }
            while (!datoCorrecto);

            //2. Declarar e inicializar un arreglo de calles
            Calle[] lasCalles = new Calle[cantidadCalles];
            Random aleatorio = new Random();

            for (int i = 0; i < lasCalles.Length; i++)
            {
                lasCalles[i] = new Calle();
                lasCalles[i].TipoDeterioro = losDeterioros[aleatorio.Next(losDeterioros.Length)];
                lasCalles[i].Longitud = aleatorio.Next(100, 501);
                lasCalles[i].TramoAfectado = 20 + aleatorio.NextDouble() * (60-20);
            }

            //3. Visualización detalle de las calles simuladas
            Console.WriteLine("\n\n*** Resultados de la simulación ***\n");

            for (int i = 0; i < lasCalles.Length; i++)
            {
                Console.WriteLine($"\nCalle No. {(i + 1)}: \nDeterioro: {lasCalles[i].TipoDeterioro}, " +
                    $"Longitud de la calle: {lasCalles[i].Longitud}, " +
                    $"tramo afectado: {lasCalles[i].TramoAfectado.ToString(".00")}% ");
            }

            //4. Visualizar los totales por cada tipo de deterioro
            int[] totalAfectacionesPorDeterioro = TotalizaAfectacionesPorDeterioro(lasCalles, losDeterioros);
            
            double[] cantidadPavimento = ObtieneCantidadCallePavimentadaPorDeterioro(lasCalles, losDeterioros);

            double[] longitudesPromedio = ObtieneLongitudPromedioTramosPorDeterioro(lasCalles, losDeterioros);

            Console.WriteLine("\n\n*** Totales por tipo de deterioro ***\n");

            for (int i = 0; i < losDeterioros.Length; i++)
            {
                Console.WriteLine($"\n{losDeterioros[i]}:\n" +
                    $"Total afectaciones: {totalAfectacionesPorDeterioro[i]} . " +
                    $"Total mts pavimentados: {cantidadPavimento[i].ToString(".00")} mts. " +
                    $"para una longitud promedio de {longitudesPromedio[i].ToString("0.00")} mts\n");
            }

        }

        static int[] TotalizaAfectacionesPorDeterioro(Calle[] lasCalles, string[] losDeterioros)
        {
            int[] totalAfectaciones = new int[losDeterioros.Length];

            for (int i = 0; i < lasCalles.Length; i++)
                for (int j = 0; j < losDeterioros.Length; j++)
                    if (lasCalles[i].TipoDeterioro == losDeterioros[j])
                        totalAfectaciones[j]++;

            return totalAfectaciones;
        }

        static double[] ObtieneCantidadCallePavimentadaPorDeterioro(Calle[] lasCalles, string[] losDeterioros)
        {
            double[] cantidades = new double[losDeterioros.Length];

            for (int i = 0; i < lasCalles.Length; i++)
                for (int j = 0; j < losDeterioros.Length; j++)
                    if (lasCalles[i].TipoDeterioro == losDeterioros[j])
                    {
                        cantidades[j] += lasCalles[i].Longitud *
                            (lasCalles[i].TramoAfectado / 100);
                    }

            return cantidades;
        }

        static double[] ObtieneLongitudPromedioTramosPorDeterioro(Calle[] lasCalles, string[] losDeterioros)
        {
            double[] longitudesPromedio = new double[losDeterioros.Length];
            int[] totalAfectaciones = TotalizaAfectacionesPorDeterioro(lasCalles, losDeterioros);
            double[] TotalPavimentoPorDeterioro = ObtieneCantidadCallePavimentadaPorDeterioro(lasCalles, losDeterioros);

            for (int i = 0; i < losDeterioros.Length; i++)
                longitudesPromedio[i] = TotalPavimentoPorDeterioro[i] / totalAfectaciones[i];

            return longitudesPromedio;
        }
    }
}

// Codigo de Prueba Unitaria

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PruebaUnitaria
{
    [TestClass]
    public class DeterioroDeCaminoTest
    {
        [TestMethod]
        public void ObtieneLongitudPromedioTramosPorDeterioro_DeberiaCalcularPromedioCorrecto()
        {

            // Arranque o Inicialización:

            var tramos = new List<Tramo>
            {
                new Tramo { Longitud = 10, Deterioro = Deterioro.Bajo },
                new Tramo { Longitud = 20, Deterioro = Deterioro.Medio },
                new Tramo { Longitud = 30, Deterioro = Deterioro.Alto },
                new Tramo { Longitud = 40, Deterioro = Deterioro.Bajo }

            };

            var deterioro = Deterioro.Bajo;
            var sut = new DeterioroDeCamino();

            // Actuación:

            var resultado = sut.ObtieneLongitudPromedioTramosPorDeterioro(tramos, deterioro);

            // Aseguramiento: 

            Assert.AreEqual(25, resultado);
        }

        [TestMethod]
        public void TotalizaAfectacionesPorDeterioro_DeberiaCalcularAfectacionesCorrectas()
        {
            var tramos = new List<Tramo>
            {
                new Tramo { Longitud = 10, Deterioro = Deterioro.Bajo },
                new Tramo { Longitud = 20, Deterioro = Deterioro.Medio },
                new Tramo { Longitud = 30, Deterioro = Deterioro.Alto }
            };

            var sut = new DeterioroDeCamino();

            // Actuación:
            var resultado = sut.TotalizaAfectacionesPorDeterioro(tramos);

            //Aseguramiento:
            Assert.AreEqual(1, resultado[Deterioro.Bajo]);
            Assert.AreEqual(1, resultado[Deterioro.Medio]);
            Assert.AreEqual(1, resultado[Deterioro.Alto]);
        }
    }

    internal class Tramo
    {
        public int Longitud { get; set; }
        public Deterioro Deterioro { get; set; }
    }

    internal enum Deterioro
    {
        Bajo,
        Medio,
        Alto
    }

    internal class DeterioroDeCamino
    {
        public double ObtieneLongitudPromedioTramosPorDeterioro(IEnumerable<Tramo> tramos, Deterioro deterioro)
        {
            double totalLongitud = 0;
            int contador = 0;

            foreach (var tramo in tramos)
            {
                if (tramo.Deterioro == deterioro)
                {
                    totalLongitud += tramo.Longitud;
                    contador++;
                }
            }

            return totalLongitud / contador;
        }

        public Dictionary<Deterioro, int> TotalizaAfectacionesPorDeterioro(IEnumerable<Tramo> tramos)
        {
            var resultado = new Dictionary<Deterioro, int>
            {
                { Deterioro.Bajo, 0 },
                { Deterioro.Medio, 0 },
                { Deterioro.Alto, 0 }
            };

            foreach (var tramo in tramos)
            {

                resultado; 

            }
