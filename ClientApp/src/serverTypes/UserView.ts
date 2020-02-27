import Leaderboard from './Leaderboard';

export default interface UserViewModel extends Leaderboard {
    isMember: boolean;
    isRecommended: boolean;
}
