export const animationDelay = (delay: string): React.CSSProperties => ({
    animationDelay: delay,
    MozAnimationDelay: delay,
    OAnimationDelay: delay,
    WebkitAnimationDelay: delay,
});
