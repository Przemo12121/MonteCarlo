#include <iostream>
#include <cstdlib>
#include <ctime>
#include <cmath>
#include <string>


using namespace std;

// translates '0' (spin down) to value of -1, and '1' (spin up) to value of 1 
int f(char c) {
    if (c == '1')
    {
        return 1;
    }
    else { //only '0' possible
        return -1;
    }
}

int main(int argc, char** argv)
{
    time_t t;
    srand(time(&t));
    int counter = 0;
    int L = std::stoi(argv[2]);     //size of array of spins
    double T = std::stod(argv[1]);  // temperature
    int mcs = std::stoi(argv[argc - 2]);    //number of duration in mcs
    int mcsh = std::stoi(argv[argc - 1]);   //variable helping in capturing intermediate step data


       int magnetism;

       //because of technical reasons, columns of the array are placed at indexes between 3 and L+3
       //though rows are naturally between 0 and L
          
       //evolving map of domains
        double deltaE;
        int i;
        int j;
        int LL = L + 3;
        for (int k = 0; k < mcs;k++,counter++) { //kroki MCS
            for (int s = 0; s < L * L; s++)
            {
                //1. Draw random spin 
                i = (rand() % L) + 3;   //+0
                j = (rand() % L);     //+0
                //distinguished cases when program calculates spin of domain at the edge
                if (i == 3 || i == LL - 1)
                {
                    if (i == 3)
                    {
                        if (j == 0 || j == L - 1)
                        {
                            if (j == 0)
                            {
                                //f function translates '0' (spin down) to value of -1, and '1' (spin up) to value of 1 
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
            //intermediate step data are send back to WPF
            if (mcsh > 0 && counter > 0 && counter%mcsh==0 && counter < mcs) {
                cout << 'c' << endl;
                for (int i = 3; i < LL; i++) {
                    cout << argv[i] << endl;
                }
                cout << counter << endl;
            }
            //calculating magnetism
            magnetism = 0;
            for (int i = 3; i < LL; i++) {
                for (int j = 0; j < L; j++) {
                    magnetism += f(argv[i][j]);
                }
            }
            //sending magnetism to WPF
            cout << 'm' << endl;
            cout << (double)magnetism / L / L << endl;
        }
        //sending final map
        cout << 'k' << endl;
        for (int i = 3; i < LL; i++) {
            cout << argv[i] << endl;
        }
    return 0;
}
