import React from 'react';

interface WidgetGridProps {
    children: React.ReactNode;
}

export const WidgetGrid: React.FC<WidgetGridProps> = ({ children }) => {
    return (
        <div className="w-full overflow-x-hidden">
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {React.Children.map(children, (child, index) => (
                    <div key={index} className="relative">
                        {child}
                    </div>
                ))}
            </div>
        </div>
    );
};
