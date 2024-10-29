namespace Socorro;

public partial class MainPage : ContentPage
{

	const int gravidade = 6;
	const int tempoEntreFrames = 20;
	bool estaMorto = false;
	double larguraJanela = 0;
	double alturaJanela = 0;
	int velocidade = 10;
	const int forcaPulo = 30;
	const int maxTempoPulando = 3;//frames
	bool estaPulando = false;
	int tempoPulando = 0;
	const int aberturaMinima = 200;
	int score = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	void AplicaGravidade()
	{
		Boom.TranslationY += gravidade;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		SoundHelper.Play("fundo.wav", true);
	}


	public async void Desenha()
	{
		while (!estaMorto)
		{
			if (estaPulando)
				AplicaPulo();
			else
				AplicaGravidade();

			GerenciaCanos();
			if (VerificaColisao())
			{
				estaMorto = true;
				SoundHelper.Play("morte.wav");
				FrameGameOver.IsVisible = true;
				break;
			}
			await Task.Delay(tempoEntreFrames);

		}
	}

	protected override void OnSizeAllocated(double w, double h)
	{
		base.OnSizeAllocated(w, h);

		larguraJanela = w;
		alturaJanela = h;
		if (h > 0)
		{
			Onac.HeightRequest = alturaJanela;
			Cano.HeightRequest = alturaJanela;
			Onac.WidthRequest = 50 * 715 / alturaJanela;
			Cano.WidthRequest = 50 * 715 / alturaJanela;
		}
	}

	private void Oi(object s, TappedEventArgs e)
	{
		FrameGameOver.IsVisible = false;
		Inicializar();
		estaMorto = false;
		Desenha();
	}

	void AplicaPulo()
	{
		Boom.TranslationY -= forcaPulo;
		tempoPulando++;
		if (tempoPulando >= maxTempoPulando)
		{
			estaPulando = false;
			tempoPulando = 0;
		}
	}

	void Inicializar()
	{
		Cano.TranslationX = -larguraJanela;
		Onac.TranslationX = -larguraJanela;
		Boom.TranslationY = 0;
		Boom.TranslationX = 0;
		score = 0;
	}

	void OnGridClicked(object s, TappedEventArgs a)
	{
		estaPulando = true;
	}

	void GerenciaCanos()
	{
		Cano.TranslationX -= velocidade;
		Onac.TranslationX -= velocidade;
		if (Onac.TranslationX < -larguraJanela)
		{
			Onac.TranslationX = 0;
			Cano.TranslationX = 0;

			var alturaMax = -(Cano.HeightRequest * 0.1);
			var alturaMin = -(Cano.HeightRequest * 0.8);

			Onac.TranslationY = Random.Shared.Next((int)alturaMin, (int)alturaMax);
			Cano.TranslationY = Onac.HeightRequest + Onac.TranslationY + aberturaMinima;

			score++;
			SoundHelper.Play("ponto.wav");
			labelScore.Text = "Canos: " + score.ToString("D3");
			inicio.Text = "Você passou por " + score.ToString("D3") + " canos!";
			if (score % 4 == 0)
				velocidade++;
		}
	}

	bool VerificaColisao()
	{
		return VerificaColisaoTeto() ||
				VerificaColisaoChao() ||
				VerificaColisaoCanoCima() ||
				VerificaColisaoCanoBaixo();
	}

	bool VerificaColisaoCano()
	{
		if (VerificaColisaoCanoBaixo() || VerificaColisaoCanoCima())
			return true;
		else
			return false;
	}

	bool VerificaColisaoCanoCima()
	{
		var posicaoHorizontalBoom = (larguraJanela - 50) - (Boom.WidthRequest / 2);
		var posicaoVerticalBoom = (alturaJanela / 2) - (Boom.HeightRequest / 2) + Boom.TranslationY;

		if (
			 posicaoHorizontalBoom >= Math.Abs(Onac.TranslationX) - Onac.WidthRequest &&
			 posicaoHorizontalBoom <= Math.Abs(Onac.TranslationX) + Onac.WidthRequest &&
			 posicaoVerticalBoom <= Onac.HeightRequest + Onac.TranslationY
		   )
			return true;
		else
			return false;
	}

	bool VerificaColisaoCanoBaixo()
	{
		var posicaoHorizontalBoom = larguraJanela - 50 - Boom.WidthRequest / 2;
		var posicaoVerticalBoom = (alturaJanela / 2) + (Boom.HeightRequest / 2) + Boom.TranslationY;

		var yMaxCano = Onac.HeightRequest + Onac.TranslationY + aberturaMinima;

		if (posicaoHorizontalBoom >= Math.Abs(Cano.TranslationX) - Cano.WidthRequest &&
			posicaoHorizontalBoom <= Math.Abs(Cano.TranslationX) + Cano.WidthRequest &&
			posicaoVerticalBoom >= yMaxCano)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	bool VerificaColisaoTeto()
	{
		var minY = -alturaJanela / 2;
		if (Boom.TranslationY <= minY)
			return true;
		else
			return false;
	}

	bool VerificaColisaoChao()
	{
		var maxY = alturaJanela / 2 - Chao.HeightRequest;
		if (Boom.TranslationY >= maxY)
			return true;
		else
			return false;
	}

}

