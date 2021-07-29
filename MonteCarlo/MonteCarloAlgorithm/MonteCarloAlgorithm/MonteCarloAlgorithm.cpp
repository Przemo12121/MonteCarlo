#include <iostream>
#include <cstdlib>
#include <ctime>
#include <cmath>
#include <string>


using namespace std;

int f(char c) {
    if (c == '1')
    {
        return 1;
    }
    else {
        return -1;
    }
}

int main(int argc, char** argv)
{
    time_t t;
    srand(time(&t));
    int counter = 0;
    int L = std::stoi(argv[2]);     //rozmiar tablicy spinów
    double T = std::stod(argv[1]);  // temperatura
    int mcs = std::stoi(argv[argc - 2]);    //ilość kroków mcs
    int mcsh = std::stoi(argv[argc - 1]);   //zmienna pomocnicza do zczytywania mapy


       int magnetyzacja;

        //uwaga! ze wzgledów technicznych, kolumny tablicy spinów są na indeksach od i = 3 do i = L+3
        // natomiast indeksy wierszów już normalnie są od j = 0 do j = L
          //Ewoluowaæ
        double deltaE;
        int i;
        int j;
        int LL = L + 3;
        for (int k = 0; k < mcs;k++,counter++) { //kroki MCS
            for (int s = 0; s < L * L; s++)
            {
                //1. Losuje spin dla losowego Sij
                i = (rand() % L) + 3;   //+0
                j = (rand() % L);     //+0
                //rozbite przypadki na sytuacje, w ktorych program bada spiny na krawedziach
                if (i == 3 || i == LL - 1)
                {
                    if (i == 3)
                    {
                        if (j == 0 || j == L - 1)
                        {
                            if (j == 0)
                            {                   //funkcja f zamienia znak 1 na wartosc 1 oraz znak 0 na wartosc -1
                                deltaE = 2 * (f(argv[i][j])) * (f(argv[i + 1][j]) + f(argv[i][j + 1]));
                            }
                            else if (j == L - 1)
                            {
                                deltaE = 2 * f(argv[i][j]) * (f(argv[i + 1][j]) + f(argv[i][j - 1]));
                            }
                        }
                        else
                        {
                            deltaE = 2 * (f(argv[i][j])) * (f(argv[i + 1][j]) + f(argv[i][j - 1]) + f(argv[i][j + 1]));
                        }
                    }
                    else if (i == LL - 1) //i=L
                    {
                        if (j == 0)
                        {
                            deltaE = 2 * f(argv[i][j]) * (f(argv[i - 1][j]) + f(argv[i][j + 1]));
                        }
                        else if (j == L - 1)
                        {
                            deltaE = 2 * f(argv[i][j]) * (f(argv[i - 1][j]) + f(argv[i][j - 1]));
                        }
                        else
                        {
                            deltaE = 2 * (f(argv[i][j])) * (f(argv[i - 1][j]) + f(argv[i][j + 1]) + f(argv[i][j - 1]));
                        }

                    }
                }
                else if (j == 0 || j == L - 1)
                {
                    if (j == 0)
                    {
                        deltaE = 2 * f(argv[i][j]) * (f(argv[i - 1][j]) + f(argv[i + 1][j]) + f(argv[i][j + 1]));
                    }
                    else if (j == L - 1)
                    {
                        deltaE = 2 * f(argv[i][j]) * (f(argv[i - 1][j]) + f(argv[i + 1][j]) + f(argv[i][j - 1]));
                    }

                }
                else
                {
                    deltaE = 2 * f(argv[i][j]) * (f(argv[i - 1][j]) + f(argv[i + 1][j]) + f(argv[i][j - 1]) + f(argv[i][j + 1]));
                }
                // delta E
                if (deltaE <= 0)
                {
                    if (argv[i][j] == '0')
                    {
                        argv[i][j] = '1';
                    }
                    else
                    {
                        argv[i][j] = '0';

                    }
                }
                else
                {
                    double x = (double)rand() / ((double)RAND_MAX + 1);
                    if (x < exp((-1)*deltaE / T))
                    {
                        //S[i][j] = -S[i][j];
                        if (argv[i][j] == '0')
                        {
                            argv[i][j] = '1';
                        }
                        else
                        {
                            argv[i][j] = '0';
                        }
                    }
                }
            }
            //tutaj jest zawarte zwracanie map przejściowych do interfejsu
            if (mcsh > 0 && counter > 0 && counter%mcsh==0 && counter < mcs) {
                cout << 'c' << endl;
                for (int i = 3; i < LL; i++) {
                    cout << argv[i] << endl;
                }
                cout << counter << endl;
            }
            //oblicanie magnetyzacji
            magnetyzacja = 0;
            for (int i = 3; i < LL; i++) {
                for (int j = 0; j < L; j++) {
                    magnetyzacja += f(argv[i][j]);
                }
            }
            //wysylanie magnetyzacji do interfejsu, zbierajacego dane do zapisania do plikow
            cout << 'm' << endl;
            cout << (double)magnetyzacja / L / L << endl;
        }
        //wysylanie koncowej mapy spinów
        cout << 'k' << endl;
        for (int i = 3; i < LL; i++) {
            cout << argv[i] << endl;
        }
    return 0;
}
