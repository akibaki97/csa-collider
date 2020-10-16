function polyroots

    lw = 3; % vonalszélesség
    p = 1.5; % pause

    M = 10.0; % együtthatók abszolút maximuma
    n = 30; % fokszám
    N = 50; % iterációk maximális száma
    eps = 1e-3; % leállási küszöb


    % polinom együtthatók
    a = (2*M*rand(n+1,1)-M) + (2*M*rand(n+1,1)-M)*I;
    % polinom deriváltjának együtthatói
    da = pder(a);

    % gyökök
    x = roots(a(end:-1:1));
    plot(real(x),imag(x),'ro','linewidth',lw)

    % gyökök becslése
    [r,R] = rootest(a);
    t = linspace(0,2*pi,100);
    hold on
    plot(r*cos(t),r*sin(t),'r-')
    plot(R*cos(t),R*sin(t),'r-')

    % kezdőállapot
    t = 2*pi*(1:n)/n;
    z = (cos(t)+sin(t)*I)*(R+r)/2;
    zplot = plot(real(z),imag(z),'bo','linewidth',lw);
    axis([-R,R,-R,R]);
    axis equal;
    %hold off
    waitforbuttonpress;

    % Aberth-módszer 
    w = zeros(1,n);
    it = 0;
    for i = 1:N
        for k = 1:n 
            frac = pval(a,z(k))/pval(da,z(k));
            w(k) = frac/(1-frac*srd(z,k));
        end
        % leállás, ha nincs lényegi változás
        if max(abs(w)) < eps,
            break
        end
        
        z = z-w;        
        
        % kirajzolás
        set(zplot,'visible','off');
        zplot = plot(real(z),imag(z),'bo','linewidth',lw);
        set(zplot,'visible','on');
        it++;
        waitforbuttonpress;
    end
    printf("roots: \n\n%s\n",disp(z'));
    printf("number of iterations: \n\n%d\n",it);

end

% polinom gyökeinek becslése 
function [r,R] = rootest(a)
    R = 1+max(abs(a(1:end-1)))/abs(a(end));
    r = 1/(1+max(abs(a(2:end)))/abs(a(1)));
end

% polinom kiértékelése
% (kb. ugyanaz, mint a "polyval")
function y = pval(a,xi)
    y = a(end);
    for k = length(a)-1:-1:1
        y = y*xi+a(k);
    end
end

% polinom deriváltjainak együtthatói
% (kb. ugyanaz, mint a "polyder")
function da = pder(a)
    n = length(a)-1;
    da = zeros(1,n);
    for k = 2:n+1
        da(k-1) = (k-1)*a(k);
    end
end

% a z vektor komponensei különbségeinek
% reciprok összege:
%
% \sum_{j \neq k} \frac{1}{z_k - z_j}

function o = srd(z,k)
    n = length(z);
    inds = [1:k-1,k+1:n];    
    o = sum(1./(z(k)-z(inds)));
end