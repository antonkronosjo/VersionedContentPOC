import { useRef, useCallback } from "react";

export function useDebouncedCallback<T extends (...args: any[]) => any>(
    callback: T,
    delay: number = 300
) {
    // useRef instead of normal const
    const timer = useRef<ReturnType<typeof setTimeout> | null>(null);

    const debouncedFn = useCallback(
        (...args: Parameters<T>) => {
            if (timer.current) clearTimeout(timer.current);

            timer.current = setTimeout(() => {
                callback(...args);
            }, delay);
        },
        [callback, delay]
    );

    return debouncedFn;
}