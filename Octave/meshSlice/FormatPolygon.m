function [x,y] = FormatPolygon(P)

n = P(1);
x = y = zeros(1,n);
for i = 2:n+1,
    x(i-1) = P(2*i-2);
    y(i-1) = P(2*i-1); 
end

end