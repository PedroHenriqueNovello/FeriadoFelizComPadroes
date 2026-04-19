/* Sistema de Benchmarking e Homologação de Hardware
Uma empresa de análise de tecnologia precisa de um software para automatizar testes de estresse em
novos hardwares que chegam ao mercado (como laptops gamers da linha Nitro, ou os novos smartphones das linhas Galaxy e iPhone).
O software deve avaliar performance, bateria e controle térmico, gerando relatórios em tempo real.*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeriadoFelizComPadroes
{
    // Singleton para gerenciar logs
    public class GerenciadorDeLogs
    {
        private static GerenciadorDeLogs _instancia;
        public static readonly object _lock = new object();

        public GerenciadorDeLogs() { }

        public static GerenciadorDeLogs Instancia
        {
            get
            {
                lock (_lock)
                {
                    if (_instancia == null)
                    {
                        _instancia = new GerenciadorDeLogs();
                    }
                    return _instancia;
                }
            }
        }

        public void Registrar(string mensagem)
        {
            Console.WriteLine($"[LOG CENTRAL] {DateTime.Now:HH:mm:ss} - {mensagem}");
        }

    }

    //Fábrica para ciração de dispositivos
    public interface IDispositivo
    {
        string Modelo { get; }
        string Processador { get; }
    }
    public class LaptopNitro : IDispositivo
    {
        public string Modelo => "Acer Nitro 5";
        public string Processador => "Intel Core i7 + RTX 4050";
    }

    public class SmartphoneGalaxy : IDispositivo
    {
        public string Modelo => "Samsung Galaxy S25";
        public string Processador => "Snapdragon 8 Gen 4";
    }

    public static class DispositivoFactory
    {
        public static IDispositivo Criar(string tipo)
        {
            return tipo.ToLower() switch
            {
                "nitro" => new LaptopNitro(),
                "galaxy" => new SmartphoneGalaxy(),
                _ => throw new ArgumentException("Dispositivo não homologado.")
            };
        }
    }

    //Adaptador para integração de sensores antigos 
    public interface ISensorTemperatura
    {
        double ObterTemperaturaCelsius();
    }

    public class SensorTermicoAntigoEUA
    {
        public double LerFahrenheit() => 194.0; // simulando uma temperatura alta
    }

    public class TermometroAdapter : ISensorTemperatura
    {
        private readonly SensorTermicoAntigoEUA _sensorLegado = new SensorTermicoAntigoEUA();

        public double ObterTemperaturaCelsius()
        {
            double fahrenheit = _sensorLegado.LerFahrenheit();
            return (fahrenheit - 32) * 5.0 / 9.0; // Convertendo para Celsius
        }
    }
    
        
    }


    internal class Program
    {

        static void Main(string[] args)
        {
        }
    }
}
