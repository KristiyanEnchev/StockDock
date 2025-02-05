import { Area, AreaChart, Line, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";
import { memo } from "react";

interface ChartDataPoint {
  date: string;
  value: number;
  MA20?: number | null;
  MA50?: number | null;
}

interface PriceChartProps {
  data: ChartDataPoint[];
  showMA?: boolean;
  symbol: string;
}

interface TooltipContentProps {
  active?: boolean;
  payload?: Array<{ value: number; payload: ChartDataPoint }>;
}

const CustomTooltip = ({ active, payload }: TooltipContentProps) => {
  if (!active || !payload || !payload.length) return null;

  const data = payload[0].payload;
  const price = data.value;

  return (
    <div className="bg-[#1e293b] p-2 border border-[#2a2d31] rounded shadow-lg">
      <p className="text-gray-400">{data.date}</p>
      <p className="text-white font-medium">${price.toFixed(2)}</p>
      {data.MA20 && <p className="text-[#3b82f6]">MA20: ${data.MA20.toFixed(2)}</p>}
      {data.MA50 && <p className="text-[#8b5cf6]">MA50: ${data.MA50.toFixed(2)}</p>}
    </div>
  );
};

export const PriceChart = memo(({
  data,
  showMA = false }: PriceChartProps) => {
  const formatDate = (date: string) => {
    const dateObj = new Date(date);
    return dateObj.toLocaleDateString();
  };

  return (
    <div className="h-80 w-full">
      <ResponsiveContainer width="100%" height="100%">
        <AreaChart data={data} margin={{ top: 10, right: 30, left: 0, bottom: 0 }}>
          <defs>
            <linearGradient id="colorValue" x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor="#22c55e" stopOpacity={0.8} />
              <stop offset="95%" stopColor="#22c55e" stopOpacity={0} />
            </linearGradient>
          </defs>
          <XAxis
            dataKey="date"
            tickFormatter={formatDate}
            tick={{ fill: '#888' }}
            axisLine={{ stroke: '#444' }}
            tickLine={{ stroke: '#444' }}
          />
          <YAxis
            tick={{ fill: '#888' }}
            axisLine={{ stroke: '#444' }}
            tickLine={{ stroke: '#444' }}
            domain={['auto', 'auto']}
          />
          <Tooltip content={<CustomTooltip />} />
          <Area
            type="monotone"
            dataKey="value"
            stroke="#22c55e"
            fillOpacity={1}
            fill="url(#colorValue)"
          />
          {showMA && (
            <>
              {data[0]?.MA20 !== undefined && (
                <Line
                  type="monotone"
                  dataKey="MA20"
                  stroke="#3b82f6"
                  dot={false}
                  strokeWidth={1.5}
                />
              )}
              {data[0]?.MA50 !== undefined && (
                <Line
                  type="monotone"
                  dataKey="MA50"
                  stroke="#8b5cf6"
                  dot={false}
                  strokeWidth={1.5}
                />
              )}
            </>
          )}
        </AreaChart>
      </ResponsiveContainer>
    </div>
  );
});

PriceChart.displayName = "PriceChart";
