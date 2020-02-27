import Category from './Category';
import Leaderboard from './Leaderboard';

export default interface UserViewModel {
    userName: string;
    email: string;
    interests: Category[];
    leaderboards: Leaderboard[];
}
