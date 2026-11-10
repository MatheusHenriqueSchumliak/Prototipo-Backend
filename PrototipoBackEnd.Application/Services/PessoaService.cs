using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Application.Common.Helpers;
using PrototipoBackEnd.Application.Dtos.Pessoa;
using PrototipoBackEnd.Application.Exceptions;
using PrototipoBackEnd.Application.Interfaces;
using PrototipoBackEnd.Application.Factories;
using PrototipoBackEnd.Domain.Interfaces;

namespace PrototipoBackEnd.Application.Services
{
    public class PessoaService : IPessoaService
    {
        #region Construtor
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IArtesaoRepository _artesaoRepository;
        private readonly IArtesanatoRepository _artesanatoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PessoaService(
            IPessoaRepository pessoaRepository,
            IUsuarioRepository usuarioRepository,
            IArtesaoRepository artesaoRepository,
            IArtesanatoRepository artesanatoRepository,
            IUnitOfWork unitOfWork)
        {
            _pessoaRepository = pessoaRepository;
            _usuarioRepository = usuarioRepository;
            _artesaoRepository = artesaoRepository;
            _artesanatoRepository = artesanatoRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        public async Task<List<PessoaDto>> BuscarTodos()
        {
            var pessoas = await _pessoaRepository.BuscarTodos();
            return pessoas.Where(p => p.IsAtivo).Select(PessoaFactory.CriarDto).ToList();
        }

        public async Task<PessoaDto> BuscarPorId(string id)
        {
            IdValidator.Validar(id);
            var pessoa = await _pessoaRepository.BuscarPorId(id);
            if (pessoa == null || !pessoa.IsAtivo)
                throw new NotFoundException("Pessoa não foi encontrada.");
            return PessoaFactory.CriarDto(pessoa);
        }

        public async Task<PessoaDto> Adicionar(PessoaDto dto)
        {
            var pessoa = PessoaFactory.CriarEntidade(dto);
            var adicionada = await _pessoaRepository.Adicionar(pessoa);
            return PessoaFactory.CriarDto(adicionada);
        }

        public async Task<PessoaDto> Atualizar(PessoaDto dto, string id)
        {
            IdValidator.Validar(id);
            var pessoaExistente = await _pessoaRepository.BuscarPorId(id);
            if (pessoaExistente == null || !pessoaExistente.IsAtivo)
                throw new NotFoundException("Pessoa não foi encontrada.");

            pessoaExistente.Atualizar(
                dto.NomeCompleto,
                dto.DataNascimento,
                EnderecoFactory.CriarEntidade(dto.Endereco),
                ContatoFactory.CriarEntidade(dto.Contato)
            );

            var atualizada = await _pessoaRepository.Atualizar(pessoaExistente, id);
            return PessoaFactory.CriarDto(atualizada);
        }

        // Soft delete em cascata: Pessoa -> Usuário, Artesão e Artesanatos, de forma atômica.
        public async Task<bool> Apagar(string id)
        {
            IdValidator.Validar(id);
            var pessoa = await _pessoaRepository.BuscarPorId(id);
            if (pessoa == null || !pessoa.IsAtivo)
                throw new NotFoundException("Pessoa não foi encontrada.");

            await _unitOfWork.ExecutarAsync(async () =>
            {
                var usuario = await _usuarioRepository.BuscarPorPessoaId(id);
                if (usuario != null)
                {
                    usuario.Inativar();
                    await _usuarioRepository.Atualizar(usuario, usuario.Id);
                }

                var artesao = await _artesaoRepository.BuscarPorPessoaId(id);
                if (artesao != null)
                {
                    var artesanatos = await _artesanatoRepository.BuscarTodosPorArtesaoId(artesao.Id);
                    foreach (var artesanato in artesanatos)
                    {
                        artesanato.Inativar();
                        await _artesanatoRepository.Atualizar(artesanato, artesanato.Id);
                    }

                    artesao.Inativar();
                    await _artesaoRepository.Atualizar(artesao, artesao.Id);
                }

                pessoa.Inativar();
                await _pessoaRepository.Atualizar(pessoa, id);
            });

            return true;
        }
    }
}
