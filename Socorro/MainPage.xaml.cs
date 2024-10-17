namespace Socorro;

public partial class MainPage : ContentPage
{

	const int gravidade = 6;
	const int tempoEntreFrames = 25;
	bool estaMorto = false;
	double larguraJanela = 0;
	double alturaJanela = 0;
	int velocidade = 7;
	const int forcaPulo = 25;
	const int maxTempoPulando = 3;
	bool estaPulando = false;
	int tempoPulando = 0;
	const int aberturaMinima = 70;
	int score = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	void AplicaGravidade()
	{
		Boom.TranslationY += gravidade;
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
				FrameGameOver.IsVisible = true;
				break;
			}
			await Task.Delay(tempoEntreFrames);

		}
	}

	void Oi(object s, TappedEventArgs e)
	{
		FrameGameOver.IsVisible = false;
		estaMorto = false;
		Inicializar();
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
		Boom.TranslationY = 0;
		score = 0;
	}

	void OnGridClicked(object s, TappedEventArgs a)
	{
		estaPulando = true;
	}


	protected override void OnSizeAllocated(double w, double h)
	{
		base.OnSizeAllocated(w, h);
		larguraJanela = w;
		alturaJanela = h;
	}

	void GerenciaCanos()
	{
		Cano.TranslationX -= velocidade;
		Onac.TranslationX -= velocidade;
		if (Onac.TranslationX < -larguraJanela)
		{
			Onac.TranslationX = 0;
			Cano.TranslationX = 0;
			var alturaMax = -10;
			var alturaMin = -Cano.HeightRequest;
			Onac.TranslationY = Random.Shared.Next((int)alturaMin, (int)alturaMax);
			Cano.TranslationY = Onac.TranslationY + aberturaMinima + Cano.HeightRequest;
			score ++;
			labelScore.Text = "Canos: " + score.ToString("D3");
			inicio.Text = "Você passou por " + score.ToString("D3") + " canos!";
		}
	}

	bool VerificaColisao()
	{
		if (!estaMorto)
		{
			if (VerificaColisaoTeto() ||
			VerificaColisaoChao())
			{
				return true;
			}
		}
		return false;
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
		else return false;
	}

}

