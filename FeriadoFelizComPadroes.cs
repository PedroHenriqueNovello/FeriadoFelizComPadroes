/* Sistema de Benchmarking e Homologação de Hardware
Uma empresa de análise de tecnologia precisa de um software para automatizar testes de estresse em
novos hardwares que chegam ao mercado (como laptops gamers da linha Nitro, ou os novos smartphones das linhas Galaxy e iPhone).
O software deve avaliar performance, bateria e controle térmico, gerando relatórios em tempo real.*/

using System;
using System.Collections.Generic;

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

    
    // Fábrica para criação de dispositivos
    
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
            switch (tipo.ToLower())
            {
                case "nitro":
                    return new LaptopNitro();

                case "galaxy":
                    return new SmartphoneGalaxy();

                default:
                    throw new ArgumentException("Dispositivo não homologado.");
            }
        }
    }


    // Adaptador para integração de sensores antigos 

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

    
    // Padrão observer para monitoramento 
    
    public interface IObserverLab
    {
        void AlertaEmergencia(string mensagem);
    }

    public class EstacaoPesquisa : IObserverLab
    {
        private string _nome;
        public EstacaoPesquisa(string nome) => _nome = nome;

        public void AlertaEmergencia(string mensagem)
        {
            Console.WriteLine($"\n[ALERTA {_nome.ToUpper()}] -> {mensagem}\n");
        }
    }

    public class ControladorTermico
    {
        private List<IObserverLab> _observadores = new List<IObserverLab>();
        public void AdicionarObservador(IObserverLab obs) => _observadores.Add(obs);

        public void VerificarTemperatura(IDispositivo disp, ISensorTemperatura sensor)
        {
            double temp = sensor.ObterTemperaturaCelsius();
            GerenciadorDeLogs.Instancia.Registrar($"Leitura térmica {disp.Modelo}: {temp}°C");

            if (temp >= 85.0)
            {
                
                foreach (var item in _observadores)
                {
                    item.AlertaEmergencia($"Risco de dano no {disp.Modelo}! Temperatura atingiu {temp}°C");
                }
            }
        }
    }

   
    // Padrão strategy para testes 
    
    public interface ITesteStrategy
    {
        void Executar(IDispositivo dispositivo);
    }

    public class TesteRenderizacao3D : ITesteStrategy
    {
        
        public void Executar(IDispositivo dispositivo)
        {
            GerenciadorDeLogs.Instancia.Registrar($"Rodando Renderização 3D em {dispositivo.Modelo} ({dispositivo.Processador})...");
        }
    }

    
    // Padrão decorator: adição de agravantes
    
    public abstract class TesteDecorator : ITesteStrategy
    {
        protected ITesteStrategy _testeBase;
        public TesteDecorator(ITesteStrategy teste) => _testeBase = teste;
        public abstract void Executar(IDispositivo dispositivo);
    }

    public class OverclockDecorator : TesteDecorator
    {
        public OverclockDecorator(ITesteStrategy teste) : base(teste)
        {
            
        }

        public override void Executar(IDispositivo dispositivo) 
        {
            GerenciadorDeLogs.Instancia.Registrar("Agravante aplicado: OVERCLOCK ATIVADO (forçando voltagem).");
            _testeBase.Executar(dispositivo);
        }
    } 

    
    // Padrão proxy para controle de acesso 
    
    public class ProxyAcessoSeguro : ITesteStrategy
    {
        private ITesteStrategy _testeReal;
        private string _credencialUsuario;

       
        public ProxyAcessoSeguro(ITesteStrategy testeReal, string credencial)
        {
            _testeReal = testeReal;
            _credencialUsuario = credencial;
        }

     
        public void Executar(IDispositivo dispositivo)
        {
            
            if (_credencialUsuario == "Pesquisador_Senior")
            {
                GerenciadorDeLogs.Instancia.Registrar("Proxy: Permissão concedida. Iniciando teste...");
                _testeReal.Executar(dispositivo);
            }
            else
            {
                GerenciadorDeLogs.Instancia.Registrar($"Proxy: ACESSO NEGADO para o usuário '{_credencialUsuario}'. Necessário nível Sênior.");
            }
        }
    }

    
    // Padrão fachada para esconder a complexidade 
    
    public class HomologacaoFacade
    {
        private ControladorTermico _controlador;
        private ISensorTemperatura _sensor;

        public HomologacaoFacade()
        {
            _controlador = new ControladorTermico();
            _controlador.AdicionarObservador(new EstacaoPesquisa("Lab UNIFESP"));
            
            _controlador.AdicionarObservador(new EstacaoPesquisa("Painel Central"));

            _sensor = new TermometroAdapter();
        }

        public void IniciarHomologacaoCompleta(string tipoDispositivo, string credencial)
        {
            Console.WriteLine("\n=== INICIANDO SISTEMA UNIFESP BENCHMARK PRO ===");

            
            IDispositivo hardware = DispositivoFactory.Criar(tipoDispositivo);

            // Strategy define o teste
            ITesteStrategy testeBase = new TesteRenderizacao3D();

            // Decorator adiciona estresse extra
            ITesteStrategy testeExtremo = new OverclockDecorator(testeBase);

            // Proxy protege a execução
            ITesteStrategy testeSeguro = new ProxyAcessoSeguro(testeExtremo, credencial);

            testeSeguro.Executar(hardware);

            // Monitoramento via adapter e observer
            if (credencial == "Pesquisador_Senior")
            {
                _controlador.VerificarTemperatura(hardware, _sensor);
            }

            Console.WriteLine("=== HOMOLOGAÇÃO FINALIZADA ===\n");
        }
    }

  
    internal class Program
    {
        static void Main(string[] args)
        {
            HomologacaoFacade facade = new HomologacaoFacade();

            facade.IniciarHomologacaoCompleta("galaxy", "Estudante_Bolsista");
            facade.IniciarHomologacaoCompleta("nitro", "Pesquisador_Senior");
        }
    }
} 
