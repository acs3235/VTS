clear all; close all; clc

% gammas = linspace(0.95,1.27,6)
% musp_vs = linspace(1.9,3.8,6) * 10 %cm^-1

% addpath([cd slash 'jsonlab']);



gs = [.9];
gammas = [0.95];
musp_vs = [1];

%%

for gam = gammas
    for g = gs
        if gam > 1 + g
            continue
        end
%         if gam < 1.0847
%             continue
%         end
        musp_v_cm = musp_vs;
    
        RunMCw1gamma1g_updated(gam,musp_v_cm,g)
    end  
end
%%
close all;


for gam = gammas
%     if gam < 1.0847
%         continue
%     end
    for g = gs
        for musp_v_cm = musp_vs
            if gam > 1 + g
                continue
            end
            
            dataname = strcat('RofRhoGamma_',num2str(gam),'_mus1_',num2str(musp_v_cm));
            [R,r] = parse_R_rho(dataname);

            fx = [.01 .025 .05:.05:1.8];
%             fx = [0:.05:1];


%             figure(1)
%             semilogy(r_log, R_log)
%             hold all;
% 
%             xlabel('distance (cm)')
%             ylabel('R (cm^-^2)')


           

%             figure(1)
%             semilogy(r_log, R_log)
%             hold all;
%             xlabel('distance (mm)')
%             ylabel('R (1/mm)')



%             figure(2)
            SFDR_1Y = ht(R,r,fx*2*pi);
%             semilogy(fx,SFDR_1Y,'DisplayName',['mu ' num2str(musp_v_cm) ' gamma ' num2str(gam) ' g ' num2str(g)])
%             hold all;
%             xlabel('f (1/mm)')
%             ylabel('R')

            save(['Results/SFDR/SFDR_mu_' num2str(musp_v_cm) '_gamma_' num2str(gam) '_g_' num2str(g) '.mat'],'SFDR_1Y');
        end
    end
end

%%
% xlabel('f (1/mm)')
% ylabel('R_M_C_M')
% figure(2)
% legend show;