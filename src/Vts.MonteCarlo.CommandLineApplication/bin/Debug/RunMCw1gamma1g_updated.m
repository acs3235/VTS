%% Run MC simulations using Modified HG (Sub-diffuse)
% 2/24/2019
% Yao Zhang

function RunMCw1gamma1g_updated(gamma,musp_vs,g1)

    %% Sampling for MHG to generate inverse CDF in data.txt
    % (You can skip this step if you made a file for the current pair of g and gamma previously)
    tic
    N=20000;
    epsilon=linspace(0,1,N); % Uniform Distribution
    A=[];

    for Num=1:size(gamma,2)
        g=(5*g1*gamma(Num) - 5*gamma(Num) + (25*g1^2*gamma(Num).^2 + 40*g1^2 - 50*g1*gamma(Num).^2 + 30*g1*gamma(Num) + 25*gamma(Num).^2 - 30*gamma(Num) + 9).^(1/2) + 3)/(10*g1);
        a=g1./g;% Right value for Beta

        costC=zeros(N,1);
        for time=1:N
            randnum=epsilon(time);
            costT = 0;
            for x=linspace(-1,1,N)
                derror = randnum-(a*(1-g*g)/(2*g)*((1+g*g-2*g*x)^(-0.5)-(1+g*g+2*g)^(-0.5))+(1-a)*(x*x*x+1)/(2));
                if derror>0
                    costT = x;
                    continue
                else
                    costT = (x + costT)/2;
                    break
                end
            end
            costC(time)=costT;
        end
        A=[A costC];
        % Plot
        Flag_Plot = 0;
        if Flag_Plot
            figure
            h0=histogram(costC,21,'Normalization','pdf');
            hold on
            %theoretical MHG phase function
            cost0=linspace(-1,1,N);
            pcost0=a*(1-g*g)./(2*(1+g*g-2*g*cost0).^(3/2))+(1-a)*3/(2)*cost0.*cost0;
            plot(cost0,pcost0);
            set(gca, 'YScale', 'log')
            xlabel('cos(\theta)')
            ylabel('probability')
            legend('Sampling (Numeric/Discretized)','MHG phase function')
            title(['g1=',num2str(g1),' gamma=',num2str(gamma(Num))])
            xlim([-1 1])
        end
    end
    toc

    fileID = fopen(['CDF_g_' num2str(g1) 'gamma_' num2str(gamma) '.txt'],'w');
    formatSpec='%8.6f \n';
    fprintf(fileID,formatSpec,A'); % Very important!!! Pay attention to the writing format!
    fclose(fileID);
    save(['CDF_g_' num2str(g1) 'gamma_' num2str(gamma) '.mat'])


    %% Always run this to replace data.txt with the desired inverse CDF file
    movefile(['CDF_g_' num2str(g1) 'gamma_' num2str(gamma) '.txt'],'data.txt')


    %% Run simulation with musp parameter sweep
    sweeptext = num2str(length(musp_vs));
    for mu_i = musp_vs
        sweeptext = strcat(sweeptext,',',num2str(mu_i));
    end
    %%
    
%     addpath('/Users/andrew/anaconda/bin:/scripts/ipynb_drop_output:/usr/local/bin:/usr/bin:/bin:/usr/sbin:/sbin:/Library/TeX/texbin:/usr/local/share/dotnet:/opt/X11/bin:/.dotnet/tools:/Library/Frameworks/Mono.framework/Versions/Current/Commands:/Library/Frameworks/Mono.framework/Versions/6.8.0/lib/mono')
    addpath('/Library/Frameworks/Mono.framework/Versions/6.8.0/include/mono-2.0/mono')
    command_text = strcat('mono mc.exe outpath=Results infile=infile_one_layer_RofRho.txt outname=RofRhoGamma_',num2str(gamma),' paramsweeplist=mus1,',sweeptext)
    
    %Run GPUMCML
    
    system(command_text) %% Random Seed %% remember to change the data.txt and the name of the program!
      
end


